#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientDeleteAccountProcessor : IProcessor<DeleteAccountRequest, bool>
{
	private readonly IRestClient _client;
	private readonly AuthStateProvider _authStateProvider;

	public ClientDeleteAccountProcessor(
		IRestClient client,
		AuthStateProvider authStateProvider)
	{
		_client = client;
		_authStateProvider = authStateProvider;
	}

	public async Task<OperationResult<bool>> Process(DeleteAccountRequest request)
	{
		var result = await _client.Delete<bool>("account", request);

		if (result.Status == OperationStatus.Success)
		{
			_authStateProvider.NotifyUserAuthentication([], false);
		}

		return result;
	}
}
