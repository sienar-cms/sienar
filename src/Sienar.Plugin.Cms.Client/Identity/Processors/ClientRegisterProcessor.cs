#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientRegisterProcessor : IProcessor<RegisterRequest, bool>
{
	private readonly IRestClient _client;

	public ClientRegisterProcessor(IRestClient client)
	{
		_client = client;
	}

	public async Task<OperationResult<bool>> Process(RegisterRequest request)
	{
		if (!request.AcceptTos)
		{
			return new OperationResult<bool>(
				OperationStatus.Unprocessable,
				false,
				CmsErrors.Account.MustAcceptTos);
		}

		return await _client.Post<bool>("account", request);
	}
}
