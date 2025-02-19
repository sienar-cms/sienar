﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class ClientAddUsertoRoleProcessor : IStatusProcessor<AddUserToRoleRequest>
{
	private readonly IRestClient _client;

	public ClientAddUsertoRoleProcessor(IRestClient client)
	{
		_client = client;
	}

	public Task<OperationResult<bool>> Process(AddUserToRoleRequest request)
		=> _client.Post<bool>("users/roles", request);
}
