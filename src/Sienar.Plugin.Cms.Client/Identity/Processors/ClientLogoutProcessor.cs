#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientLogoutProcessor : IProcessor<LogoutRequest, bool>
{
	private readonly IRestClient _client;
	private readonly AuthStateProvider _authStateProvider;

	public ClientLogoutProcessor(
		IRestClient client,
		AuthStateProvider authStateProvider)
	{
		_client = client;
		_authStateProvider = authStateProvider;
	}

	public async Task<OperationResult<bool>> Process(LogoutRequest request)
	{
		var loggedOutResult = await _client.Delete<bool>("account/login", request);

		if (loggedOutResult.Status is OperationStatus.Success)
		{
			_authStateProvider.NotifyUserAuthentication([], false);
			await _client.RefreshCsrfToken();
		}

		return loggedOutResult;
	}
}
