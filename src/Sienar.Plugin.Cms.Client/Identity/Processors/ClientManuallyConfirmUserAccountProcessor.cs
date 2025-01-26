#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientManuallyConfirmUserAccountProcessor : IProcessor<ManuallyConfirmUserAccountRequest, bool>
{
	private readonly IRestClient _client;

	public ClientManuallyConfirmUserAccountProcessor(IRestClient client)
	{
		_client = client;
	}

	public Task<OperationResult<bool>> Process(
		ManuallyConfirmUserAccountRequest request)
		=> _client.Patch<bool>("users/confirm", request);
}
