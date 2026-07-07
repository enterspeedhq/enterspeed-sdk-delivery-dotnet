using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using Enterspeed.Delivery.Sdk.Api.Connection;
using Enterspeed.Delivery.Sdk.Api.Providers;
using Enterspeed.Delivery.Sdk.Configuration;

namespace Enterspeed.Delivery.Sdk.Domain.Connection
{
    /// <summary>
    /// Thread-safe holder of the SDK's shared <see cref="HttpClient"/>, intended to live as a singleton.
    /// Configuration (<see cref="EnterspeedDeliveryConfiguration.BaseUrl"/> and
    /// <see cref="EnterspeedDeliveryConfiguration.ConnectionTimeout"/>) is captured once at construction
    /// and is not re-read afterwards.
    /// </summary>
    public sealed class EnterspeedDeliveryConnection : IEnterspeedDeliveryConnection, IDisposable
    {
        private readonly object _connectionLock = new object();
        private readonly int _refreshIntervalSeconds;
        private readonly Func<HttpMessageHandler> _handlerFactory;
#if !NETCOREAPP2_1_OR_GREATER
        private readonly Func<long> _timestampProvider;
        private readonly long _freshnessIntervalTicks;
#endif
        private bool _disposed;

        // The client (and, on netstandard2.0, its creation timestamp) is published as one immutable
        // snapshot swapped by reference. The lock-free fast path reads the field with Volatile.Read and
        // writers publish with Volatile.Write only after the snapshot is fully configured, so a reader
        // can never observe a half-configured client or a client from one generation paired with another
        // generation's timestamp. Storing the client and timestamp as two separate fields allowed a
        // concurrent refresh or Flush() to interleave between the reads (observed as
        // NullReferenceException inside DeliveryApiResponse under production load).
        private ClientSnapshot _current;

        public EnterspeedDeliveryConnection(IEnterspeedConfigurationProvider configurationProvider)
            : this(configurationProvider, null)
        {
        }

        // Internal seams for deterministic tests, invisible to package consumers:
        // - timestampProvider replaces Stopwatch.GetTimestamp for the netstandard2.0 timed refresh
        //   (ignored on modern targets, which have no timed refresh);
        // - handlerFactory replaces the HttpMessageHandler so service-level tests can run offline.
        internal EnterspeedDeliveryConnection(
            IEnterspeedConfigurationProvider configurationProvider,
            Func<long> timestampProvider,
            Func<HttpMessageHandler> handlerFactory = null)
        {
            if (configurationProvider == null)
            {
                throw new ArgumentNullException(nameof(configurationProvider));
            }

            BaseUrl = configurationProvider.Configuration?.BaseUrl;

            // Values <= 0 are treated as 1 second: a zero/negative value used to mean "recreate the
            // client on every access", which defeats connection pooling and allocates an unbounded
            // number of clients at high request rates. Flooring instead of throwing keeps every
            // previously accepted configuration value accepted.
            var connectionTimeout = configurationProvider.Configuration?.ConnectionTimeout ?? 60;
            _refreshIntervalSeconds = Math.Max(connectionTimeout, 1);

#if !NETCOREAPP2_1_OR_GREATER
            _timestampProvider = timestampProvider ?? Stopwatch.GetTimestamp;

            // Guard the tick conversion against overflow (e.g. ConnectionTimeout = int.MaxValue on a
            // high-frequency timer): a silent overflow would go negative and mean "never fresh",
            // resurrecting the recreate-on-every-access behaviour the floor above exists to prevent.
            _freshnessIntervalTicks = _refreshIntervalSeconds > long.MaxValue / Stopwatch.Frequency
                ? long.MaxValue
                : _refreshIntervalSeconds * Stopwatch.Frequency;
#endif
            _handlerFactory = handlerFactory;
        }

        private string BaseUrl { get; }

        public void Dispose()
        {
            lock (_connectionLock)
            {
                if (_disposed)
                {
                    // Dispose must be idempotent.
                    return;
                }

                // End-of-life disposal is intentional here, unlike the mid-life client replacement in
                // Connect(): the owner is declaring that no further requests will be made.
                _current?.Client.Dispose();
                Volatile.Write(ref _current, null);
                _disposed = true;
            }
        }

        /// <inheritdoc />
        public HttpClient HttpClientConnection
        {
            get
            {
                // Lock-free fast path: a single volatile read of the current snapshot. Volatile.Read
                // prevents the read from being cached/hoisted; the reference itself is immutable once
                // published.
                var snapshot = Volatile.Read(ref _current);

                if (IsFresh(snapshot))
                {
                    return snapshot.Client;
                }

                lock (_connectionLock)
                {
                    if (_disposed)
                    {
                        throw new ObjectDisposedException(nameof(EnterspeedDeliveryConnection));
                    }

                    snapshot = _current;
                    if (!IsFresh(snapshot))
                    {
                        snapshot = Connect();
                    }

                    return snapshot.Client;
                }
            }
        }

        /// <inheritdoc />
        public void Flush()
        {
            lock (_connectionLock)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(nameof(EnterspeedDeliveryConnection));
                }

                // The discarded client is intentionally left to in-flight requests; see Connect() for
                // the disposal rationale.
                Volatile.Write(ref _current, null);
            }
        }

        private bool IsFresh(ClientSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return false;
            }

#if NETCOREAPP2_1_OR_GREATER
            // One rotation mechanism per target: on modern runtimes the client's identity is stable
            // between Flush() calls, and connection-level rotation (the DNS-refresh concern) is owned by
            // SocketsHttpHandler.PooledConnectionLifetime configured in Connect(). A timed client
            // teardown on top of that would only duplicate what the pool already does.
            return true;
#else
            // netstandard2.0 has no SocketsHttpHandler, so timed client recreation is the only DNS
            // rotation mechanism available without assuming a DI container/IHttpClientFactory. Elapsed
            // time is measured with the monotonic Stopwatch timestamp rather than DateTime.UtcNow:
            // wall-clock steps (NTP) must not stretch or shrink the client's lifetime.
            return _timestampProvider() - snapshot.CreatedTimestamp < _freshnessIntervalTicks;
#endif
        }

        private ClientSnapshot Connect()
        {
            if (string.IsNullOrWhiteSpace(BaseUrl))
            {
                throw new ConfigurationException(nameof(BaseUrl));
            }

            HttpClient httpClient;

            var handler = _handlerFactory?.Invoke();

#if NETCOREAPP2_1_OR_GREATER
            // PooledConnectionLifetime is Microsoft's sanctioned DNS-rotation mechanism for a long-lived
            // HttpClient; ConnectionTimeout configures it (previously it was hardcoded to 60 seconds and
            // the configured value was silently ignored here).
            handler = handler ?? new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromSeconds(_refreshIntervalSeconds)
            };

            httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
#else
            httpClient = handler != null
                ? new HttpClient(handler)
                : new HttpClient();

            httpClient.BaseAddress = new Uri(BaseUrl);
#endif

            // Fully configure the client BEFORE publishing it: publishing first and mutating afterwards
            // let concurrent callers observe a half-configured client (missing Accept header) or race the
            // non-thread-safe header collection.
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            // The previous client (if any) is intentionally not disposed here: in-flight requests may
            // still own it, and disposing it under them closes their connections and cancels their
            // requests. Its release is therefore non-deterministic (left to garbage collection), but the
            // abandoned handler's own pool drains via PooledConnectionLifetime/PooledConnectionIdleTimeout,
            // and abandonment is rare: only on Flush() on modern targets, and at most once per refresh
            // interval (floored at 1 second) on netstandard2.0. Factory-style ref-counted deferred
            // disposal (what IHttpClientFactory does) was considered and rejected as disproportionate
            // for this surface.
#if NETCOREAPP2_1_OR_GREATER
            var snapshot = new ClientSnapshot(httpClient);
#else
            var snapshot = new ClientSnapshot(httpClient, _timestampProvider());
#endif
            Volatile.Write(ref _current, snapshot);

            return snapshot;
        }

        // Must remain a sealed CLASS: single-reference publication is load-bearing for the lock-free
        // fast path. As a struct the members could tear when read without the lock.
        private sealed class ClientSnapshot
        {
#if NETCOREAPP2_1_OR_GREATER
            public ClientSnapshot(HttpClient client)
            {
                Client = client;
            }
#else
            public ClientSnapshot(HttpClient client, long createdTimestamp)
            {
                Client = client;
                CreatedTimestamp = createdTimestamp;
            }

            public long CreatedTimestamp { get; }
#endif

            public HttpClient Client { get; }
        }
    }
}
