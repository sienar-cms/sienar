#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientChangePasswordProcessor : IStatusProcessor<ChangePasswordRequest>
{
	private readonly IRestClient _client;

	public ClientChangePasswordProcessor(IRestClient client)
	{
		_client = client;
	}

	public Task<OperationResult<bool>> Process(ChangePasswordRequest request)
		=> _client.Patch<bool>("account/change-password", request);
}
