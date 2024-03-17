using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity.Processors;

public class UserRoleChangeProcessor : DbService<SienarUser>,
	IProcessor<AddUserToRoleRequest, bool>,
	IProcessor<RemoveUserFromRoleRequest, bool>
{
	private SienarUser? _user;
	private SienarRole? _role;

	/// <inheritdoc />
	public UserRoleChangeProcessor(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier)
		: base(context, logger, notifier) {}

#region AddUserToRoleRequest

	/// <inheritdoc />
	async Task<HookResult<bool>> IProcessor<AddUserToRoleRequest, bool>.Process(AddUserToRoleRequest request)
	{
		_user = await GetSienarUserWithRoles(request.UserId);
		if (_user is null)
		{
			Notifier.Error(ErrorMessages.Account.NotFound);
			return new(HookStatus.NotFound);
		}

		if (_user.Roles.Any(r => r.Id == request.RoleId))
		{
			Notifier.Warning(ErrorMessages.Account.AccountAlreadyInRole);
			return new(HookStatus.Unprocessable);
		}

		_role = await Context
			.Set<SienarRole>()
			.FindAsync(request.RoleId);
		if (_role is null)
		{
			Notifier.Error(ErrorMessages.Roles.NotFound);
			return new(HookStatus.NotFound);
		}

		_user.Roles.Add(_role);
		await Context.SaveChangesAsync();

		return new(HookStatus.Success, true);
	}

	/// <inheritdoc />
	void IProcessor<AddUserToRoleRequest, bool>.NotifySuccess()
	{
		Notifier.Success($"User {_user?.Username} added to role {_role?.Name}");
	}

	/// <inheritdoc />
	void IProcessor<AddUserToRoleRequest, bool>.NotifyFailure()
	{
		Notifier.Error("An unknown error occurred while adding user to role");
	}

	/// <inheritdoc />
	void IProcessor<AddUserToRoleRequest, bool>.NotifyNoPermission()
	{
		Notifier.Error("You do not have permission to add users to roles");
	}

#endregion

#region RemoveUserFromRoleRequest

	/// <inheritdoc />
	async Task<HookResult<bool>> IProcessor<RemoveUserFromRoleRequest, bool>.Process(RemoveUserFromRoleRequest request)
	{
		_user = await GetSienarUserWithRoles(request.UserId);
		if (_user is null)
		{
			Notifier.Error(ErrorMessages.Account.NotFound);
			return new(HookStatus.NotFound);
		}

		_role = _user.Roles.FirstOrDefault(r => r.Id == request.RoleId);
		if (_role is null)
		{
			Notifier.Warning(ErrorMessages.Account.AccountNotInRole);
			return new(HookStatus.Unprocessable);
		}

		_user.Roles.Remove(_role);
		await Context.SaveChangesAsync();

		return new(HookStatus.Success, true);
	}

	/// <inheritdoc />
	void IProcessor<RemoveUserFromRoleRequest, bool>.NotifySuccess()
	{
		Notifier.Success($"User {_user?.Username} removed from role {_role?.Name}");
	}

	/// <inheritdoc />
	void IProcessor<RemoveUserFromRoleRequest, bool>.NotifyFailure()
	{
		Notifier.Error("An unknown error occurred while removing user from role");
	}

	/// <inheritdoc />
	void IProcessor<RemoveUserFromRoleRequest, bool>.NotifyNoPermission()
	{
		Notifier.Error("You do not have permission to remove users from roles");
	}

#endregion

	private Task<SienarUser?> GetSienarUserWithRoles(Guid id)
		=> EntitySet
			.Include(u => u.Roles)
			.FirstOrDefaultAsync(u => u.Id == id);
}