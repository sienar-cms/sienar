using System.Threading.Tasks;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientConfirmAccountProcessor : IProcessor<ConfirmAccountRequest, bool>
{
	private readonly IRestClient _client;

	public ClientConfirmAccountProcessor(IRestClient client)
	{
		_client = client;
	}

	public Task<OperationResult<bool>> Process(ConfirmAccountRequest request)
		=> _client.Post<bool>("account/confirm", request);
}
