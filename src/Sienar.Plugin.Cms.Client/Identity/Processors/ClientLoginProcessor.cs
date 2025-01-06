#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Identity.Results;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientLoginProcessor : IProcessor<LoginRequest, LoginResult>
{
	private readonly IRestClient _client;
	private readonly ILogger<ClientLoginProcessor> _logger;

	public ClientLoginProcessor(
		IRestClient client,
		ILogger<ClientLoginProcessor> logger)
	{
		_client = client;
		_logger = logger;
	}

	public async Task<OperationResult<LoginResult?>> Process(LoginRequest request)
	{
		var result = await _client.Post<LoginResult?>("account/login", request);

		if (result.Status == OperationStatus.Success)
		{
			// do something to initiate login
			_logger.LogInformation("Login was successful");
		}
		else
		{
			_logger.LogError(
				"Login was not successful: {status}/{message}",
				result.Status,
				result.Message ?? "(none)");
		}

		return result;
	}
}
