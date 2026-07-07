using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Enterspeed.Delivery.Sdk.Api.Providers;
using Enterspeed.Delivery.Sdk.Configuration;
using Enterspeed.Delivery.Sdk.Domain.Connection;
using Xunit;

namespace Enterspeed.Delivery.Sdk.Tests.Domain.Connection
{
    public class EnterspeedDeliveryConnectionTests
    {
        private const string BaseUrl = "https://delivery.enterspeed.com";

        private sealed class StaticConfigurationProvider : IEnterspeedConfigurationProvider
        {
            public StaticConfigurationProvider(int connectionTimeout)
            {
                Configuration = new EnterspeedDeliveryConfiguration
                {
                    BaseUrl = BaseUrl,
                    ConnectionTimeout = connectionTimeout
                };
            }

            public EnterspeedDeliveryConfiguration Configuration { get; }
        }

        /// <summary>
        /// Deterministic fake for the connection's internal Stopwatch-timestamp seam. Seconds are
        /// converted with the same <see cref="Stopwatch.Frequency" /> the production freshness check
        /// uses, so the tests express time in seconds without duplicating tick arithmetic.
        /// </summary>
        private sealed class FakeTimestamp
        {
            private long _ticks;

            public long Read() => Volatile.Read(ref _ticks);

            public void AdvanceSeconds(double seconds) =>
                Interlocked.Add(ref _ticks, (long)(seconds * Stopwatch.Frequency));
        }

        [Fact]
        public void HttpClientConnection_OnFirstAccess_ReturnsFullyConfiguredClient()
        {
            using var connection = new EnterspeedDeliveryConnection(new StaticConfigurationProvider(60));

            var client = connection.HttpClientConnection;

            Assert.NotNull(client);
            Assert.Equal(new Uri(BaseUrl), client.BaseAddress);
            Assert.Contains("application/json", client.DefaultRequestHeaders.GetValues("Accept"));
        }

        [Fact]
        public void HttpClientConnection_AfterFlush_ReturnsNewFullyConfiguredClient()
        {
            using var connection = new EnterspeedDeliveryConnection(new StaticConfigurationProvider(60));

            var first = connection.HttpClientConnection;
            connection.Flush();
            var second = connection.HttpClientConnection;

            Assert.NotNull(second);
            Assert.NotSame(first, second);
            Assert.Equal(new Uri(BaseUrl), second.BaseAddress);
            Assert.Contains("application/json", second.DefaultRequestHeaders.GetValues("Accept"));
            first.Dispose();
        }

        [Fact]
        public void Dispose_IsIdempotent_AndSubsequentAccessThrows()
        {
            var connection = new EnterspeedDeliveryConnection(new StaticConfigurationProvider(60));
            _ = connection.HttpClientConnection;

            connection.Dispose();
            connection.Dispose();

            Assert.Throws<ObjectDisposedException>(() => connection.HttpClientConnection);
            Assert.Throws<ObjectDisposedException>(() => connection.Flush());
        }

#if NET48
        // These tests run against the SDK's netstandard2.0 asset, which is the only target with timed
        // client recreation (no SocketsHttpHandler there, so recreating the client is its DNS-rotation
        // mechanism). Time is driven through the internal timestamp seam — no sleeps, no wall clock.

        [Fact]
        public void HttpClientConnection_WhileWithinRefreshInterval_ReturnsSameClient()
        {
            var clock = new FakeTimestamp();
            using var connection = new EnterspeedDeliveryConnection(
                new StaticConfigurationProvider(60), clock.Read);

            var first = connection.HttpClientConnection;
            clock.AdvanceSeconds(59);
            var second = connection.HttpClientConnection;

            Assert.Same(first, second);
        }

        [Fact]
        public void HttpClientConnection_AfterRefreshInterval_RecreatesFullyConfiguredClient()
        {
            var clock = new FakeTimestamp();
            using var connection = new EnterspeedDeliveryConnection(
                new StaticConfigurationProvider(60), clock.Read);

            var first = connection.HttpClientConnection;
            clock.AdvanceSeconds(61);
            var second = connection.HttpClientConnection;

            Assert.NotSame(first, second);
            Assert.Equal(new Uri(BaseUrl), second.BaseAddress);
            Assert.Contains("application/json", second.DefaultRequestHeaders.GetValues("Accept"));
            first.Dispose();
        }

        [Fact]
        public void HttpClientConnection_ZeroConnectionTimeout_IsFlooredToOneSecond()
        {
            // ConnectionTimeout <= 0 used to mean "recreate on every access", which allocates an
            // unbounded number of clients under load. The floor turns it into a one-second interval.
            var clock = new FakeTimestamp();
            using var connection = new EnterspeedDeliveryConnection(
                new StaticConfigurationProvider(0), clock.Read);

            var first = connection.HttpClientConnection;
            clock.AdvanceSeconds(0.5);
            var withinFlooredInterval = connection.HttpClientConnection;
            clock.AdvanceSeconds(1.0);
            var afterFlooredInterval = connection.HttpClientConnection;

            Assert.Same(first, withinFlooredInterval);
            Assert.NotSame(first, afterFlooredInterval);
            first.Dispose();
        }
#else
        // On modern targets the client's identity is stable between Flush() calls: DNS rotation is owned
        // by SocketsHttpHandler.PooledConnectionLifetime, not by timed client teardown. The advancing
        // fake clock proves the timestamp seam is genuinely unused there.

        [Fact]
        public void HttpClientConnection_ClientIdentityIsStableOverTime()
        {
            var clock = new FakeTimestamp();
            using var connection = new EnterspeedDeliveryConnection(
                new StaticConfigurationProvider(1), clock.Read);

            var first = connection.HttpClientConnection;
            clock.AdvanceSeconds(TimeSpan.FromDays(365).TotalSeconds);
            var second = connection.HttpClientConnection;

            Assert.Same(first, second);
        }
#endif

        [Fact]
        public async Task HttpClientConnection_ConcurrentReadsAndFlushes_NeverNullOrHalfConfigured()
        {
            const int readersCount = 4;
            const int readsPerReader = 5_000;
            const int flushCount = 500;

            // ConnectionTimeout = int.MaxValue makes Flush() the only invalidation trigger, so client
            // creation is bounded by flushCount + 1 regardless of loop speed — no unbounded allocation.
            using var connection = new EnterspeedDeliveryConnection(new StaticConfigurationProvider(int.MaxValue));
            var created = new ConcurrentDictionary<HttpClient, byte>();
            var failures = new ConcurrentQueue<string>();
            using var barrier = new Barrier(readersCount + 1);

            // LongRunning: dedicated threads, so blocking on the barrier cannot starve the thread pool
            // on low-core CI agents.
            var readers = Enumerable.Range(0, readersCount).Select(_ => Task.Factory.StartNew(
                () =>
                {
                    barrier.SignalAndWait();
                    for (var i = 0; i < readsPerReader; i++)
                    {
                        var client = connection.HttpClientConnection;
                        if (client == null)
                        {
                            failures.Enqueue("HttpClientConnection returned null");
                            return;
                        }

                        created.TryAdd(client, 0);

                        // The Accept header must always be present: a client may never be observable half-configured.
                        if (!client.DefaultRequestHeaders.Contains("Accept"))
                        {
                            failures.Enqueue("Client observed without Accept header");
                            return;
                        }
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default)).ToArray();

            var flusher = Task.Factory.StartNew(
                () =>
                {
                    barrier.SignalAndWait();
                    for (var i = 0; i < flushCount; i++)
                    {
                        connection.Flush();
                        Thread.Yield();
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            await Task.WhenAll(readers.Append(flusher));

            Assert.Empty(failures);

            // Boundedness, not just absence of nulls: at most one client per flush plus the initial one.
            Assert.InRange(created.Count, 1, flushCount + 1);

            // Dispose every client this test forced into existence — no finalizer flood.
            foreach (var client in created.Keys)
            {
                client.Dispose();
            }
        }
    }
}
