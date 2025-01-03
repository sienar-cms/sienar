using System.Net.Http;
using System.Threading.Tasks;

namespace Sienar.Infrastructure;

/// <summary>
/// This auth client's methods do nothing because browser clients using cookie auth cannot refresh their auth in the way a token-based client might do
/// </summary>
public class CookieRestAuthBrowserClient : IRestAuthClient
{
	/// <inheritdoc />
	/// <remarks>
	/// The WASM client doesn't refresh the authentication at all because WASM apps using default Sienar configuration are authenticated using cookies
	/// </remarks>
	public Task<bool> RefreshAuthentication() => Task.FromResult(false);

	/// <inheritdoc />
	/// <remarks>
	/// The WASM client doesn't use a manually-included authentication cheme because WASM apps using default Sienar configuration are authenticated using cookies 
	/// </remarks>
	public Task AddAuthentication(HttpRequestMessage request) => Task.CompletedTask;
}
