using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Sienar.Services;

public class WasmRestAuthClient : IRestAuthClient
{
	private readonly HttpClient _client;
	private string? _token;

	public WasmRestAuthClient(HttpClient client) => _client = client;

	/// <inheritdoc />
	public async Task<bool> RefreshAuthorization()
	{
		var tokenRequest = new HttpRequestMessage(
			HttpMethod.Get,
			"/api/account/token");
		var response = await _client.SendAsync(tokenRequest);

		if (response.StatusCode == HttpStatusCode.Unauthorized)
		{
			return false;
		}

		if (response.Headers.TryGetValues(SienarConstants.AccessTokenHeader, out var values))
		{
			_token = values.FirstOrDefault();
			return !string.IsNullOrEmpty(_token);
		}

		return false;
	}

	/// <inheritdoc />
	public Task AddAuthorization(HttpRequestMessage request)
	{
		if (!string.IsNullOrEmpty(_token))
		{
			request.Headers.Authorization = new AuthenticationHeaderValue(
				"Bearer",
				_token);
		}

		return Task.CompletedTask;
	}
}