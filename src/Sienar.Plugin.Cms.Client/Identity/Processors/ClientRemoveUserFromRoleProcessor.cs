#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientRemoveUserFromRoleProcessor : IProcessor<RemoveUserFromRoleRequest, bool>
{
	private readonly IRestClient _client;

	public ClientRemoveUserFromRoleProcessor(IRestClient client)
	{
		_client = client;
	}

	public Task<OperationResult<bool>> Process(RemoveUserFromRoleRequest request)
		=> _client.Delete<bool>("users/roles", request);
}
