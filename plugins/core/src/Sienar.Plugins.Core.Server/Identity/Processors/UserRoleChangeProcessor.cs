#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using System.Threading.Tasks;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Data;
using Sienar.Identity.Data;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class UserRoleChangeProcessor
	: IStatusProcessor<AddUserToRoleRequest>,
		IStatusProcessor<RemoveUserFromRoleRequest>
{
	private readonly IUserRepository _userRepository;
	private readonly IRepository<SienarRole> _roleRepository;

	public UserRoleChangeProcessor(
		IUserRepository userRepository,
		IRepository<SienarRole> roleRepository)
	{
		_userRepository = userRepository;
		_roleRepository = roleRepository;
	}

	async Task<OperationResult<bool>> IStatusProcessor<AddUserToRoleRequest>.Process(AddUserToRoleRequest request)
	{
		var user = await GetSienarUserWithRoles(request.UserId);
		if (user is null)
		{
			return new(OperationStatus.NotFound, message: CmsErrors.Account.NotFound);
		}

		if (user.Roles.Any(r => r.Id == request.RoleId))
		{
			return new(OperationStatus.Unprocessable, message: CmsErrors.Account.AccountAlreadyInRole);
		}

		var role = await _roleRepository.Read(request.RoleId);
		if (role is null)
		{
			return new(OperationStatus.NotFound, message: CmsErrors.Roles.NotFound);
		}

		user.Roles.Add(role);
		return await _userRepository.Update(user)
			? new(OperationStatus.Success, true, $"User {user.Username} added to role {role.Name}")
			: new(OperationStatus.Unknown, false, StatusMessages.Database.QueryFailed);
	}

	async Task<OperationResult<bool>> IStatusProcessor<RemoveUserFromRoleRequest>.Process(RemoveUserFromRoleRequest request)
	{
		var user = await GetSienarUserWithRoles(request.UserId);
		if (user is null)
		{
			return new(OperationStatus.NotFound, message: CmsErrors.Account.NotFound);
		}

		var role = user.Roles.FirstOrDefault(r => r.Id == request.RoleId);
		if (role is null)
		{
			return new(OperationStatus.Unprocessable, message: CmsErrors.Account.AccountNotInRole);
		}

		user.Roles.Remove(role);
		await _userRepository.Update(user);

		return new(OperationStatus.Success, true, $"User {user.Username} removed from role {role.Name}");
	}

	private Task<SienarUser?> GetSienarUserWithRoles(int id)
		=> _userRepository.Read(id, SienarUserFilterFactory.WithRoles());
}