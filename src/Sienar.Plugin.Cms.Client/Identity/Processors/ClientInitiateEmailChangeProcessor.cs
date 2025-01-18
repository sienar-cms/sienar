#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientInitiateEmailChangeProcessor : IProcessor<InitiateEmailChangeRequest, bool>
{
	private readonly IRestClient _client;

	public ClientInitiateEmailChangeProcessor(IRestClient client)
	{
		_client = client;
	}

	public Task<OperationResult<bool>> Process(InitiateEmailChangeRequest request)
		=> _client.Post<bool>("account/change-email", request);
}