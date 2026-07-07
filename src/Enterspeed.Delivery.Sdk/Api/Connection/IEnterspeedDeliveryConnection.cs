using System.Net.Http;

namespace Enterspeed.Delivery.Sdk.Api.Connection
{
    public interface IEnterspeedDeliveryConnection
    {
        /// <summary>
        /// Gets the SDK-owned <see cref="HttpClient"/>.
        /// The returned instance is shared between all callers. Only the Send-family methods
        /// (<c>SendAsync</c>, <c>GetAsync</c>, <c>PostAsync</c>, ...) are thread safe on it — do not
        /// mutate <see cref="HttpClient.DefaultRequestHeaders"/>, <see cref="HttpClient.BaseAddress"/>
        /// or other properties: <c>DefaultRequestHeaders</c> is not thread safe and must not be modified
        /// while requests are outstanding. The instance can be replaced (by <see cref="Flush"/>, or by
        /// the timed refresh on .NET Framework/netstandard2.0), and any customisations made to a
        /// replaced instance are not preserved.
        /// </summary>
        HttpClient HttpClientConnection { get; }

        /// <summary>
        /// Resets the connection on demand: the current <see cref="HttpClient"/> is discarded and the
        /// next access to <see cref="HttpClientConnection"/> creates a fresh client with a fresh
        /// connection pool, forcing DNS re-resolution. Configuration is not re-read — it is captured
        /// once when the connection is constructed.
        /// </summary>
        void Flush();
    }
}
