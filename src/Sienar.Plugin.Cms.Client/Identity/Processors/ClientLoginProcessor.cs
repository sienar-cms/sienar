#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Extensions;
using Sienar.Identity.Requests;
using Sienar.Identity.Results;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientLoginProcessor : IProcessor<LoginRequest, LoginResult>
{
	private readonly IRestClient _client;
	private readonly IResultProcessor<AccountDataResult> _loadUserDataProcessor;

	public ClientLoginProcessor(
		IRestClient client,
		IResultProcessor<AccountDataResult> loadUserDataProcessor)
	{
		_client = client;
		_loadUserDataProcessor = loadUserDataProcessor;
	}

	public async Task<OperationResult<LoginResult?>> Process(LoginRequest request)
	{
		var result = await _client.Post<LoginResult?>("account/login", request);

		if (result.Status == OperationStatus.Success)
		{
			await _loadUserDataProcessor.Process();
			await _client.RefreshCsrfToken();
		}

		return result;
	}
}
