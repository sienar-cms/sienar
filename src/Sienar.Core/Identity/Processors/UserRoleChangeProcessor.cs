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
	IProcessor<AddUsertoRoleRequest>,
	IProcessor<RemoveUserFromRoleRequest>
{
	private SienarUser? _user;
	private SienarRole? _role;

	/// <inheritdoc />
	public UserRoleChangeProcessor(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier)
		: base(contextAccessor, logger, notifier) {}

#region AddUserToRoleRequest

	/// <inheritdoc />
	async Task<HookStatus> IProcessor<AddUsertoRoleRequest>.Process(AddUsertoRoleRequest request)
	{
		_user = await GetSienarUserWithRoles(request.UserId);
		if (_user is null)
		{
			Notifier.Error(ErrorMessages.Account.NotFound);
			return HookStatus.NotFound;
		}

		if (_user.Roles.Any(r => r.Id == request.RoleId))
		{
			Notifier.Warning(ErrorMessages.Account.AccountAlreadyInRole);
			return HookStatus.Unprocessable;
		}

		_role = await Context
			.Set<SienarRole>()
			.FindAsync(request.RoleId);
		if (_role is null)
		{
			Notifier.Error(ErrorMessages.Roles.NotFound);
			return HookStatus.NotFound;
		}

		_user.Roles.Add(_role);
		await Context.SaveChangesAsync();

		return HookStatus.Success;
	}

	/// <inheritdoc />
	void IProcessor<AddUsertoRoleRequest>.NotifySuccess()
	{
		Notifier.Success($"User {_user?.Username} added to role {_role?.Name}");
	}

	/// <inheritdoc />
	void IProcessor<AddUsertoRoleRequest>.NotifyBeforeHookFailure()
	{
		Notifier.Error("Unable to add user to role");
	}

	/// <inheritdoc />
	void IProcessor<AddUsertoRoleRequest>.NotifyProcessFailure()
	{
		Notifier.Error("An unknown error occurred while adding user to role");
	}

	/// <inheritdoc />
	void IProcessor<AddUsertoRoleRequest>.NotifyAfterHookFailure()
	{
		Notifier.Warning($"User {_user?.Username} added to role {_role?.Name} successfully, but a third party plugin failed to execute");
	}

#endregion

#region RemoveUserFromRoleRequest

	/// <inheritdoc />
	async Task<HookStatus> IProcessor<RemoveUserFromRoleRequest>.Process(RemoveUserFromRoleRequest request)
	{
		_user = await GetSienarUserWithRoles(request.UserId);
		if (_user is null)
		{
			Notifier.Error(ErrorMessages.Account.NotFound);
			return HookStatus.NotFound;
		}

		_role = _user.Roles.FirstOrDefault(r => r.Id == request.RoleId);
		if (_role is null)
		{
			Notifier.Warning(ErrorMessages.Account.AccountNotInRole);
			return HookStatus.Unprocessable;
		}

		_user.Roles.Remove(_role);
		await Context.SaveChangesAsync();

		return HookStatus.Success;
	}

	/// <inheritdoc />
	void IProcessor<RemoveUserFromRoleRequest>.NotifySuccess()
	{
		Notifier.Success($"User {_user?.Username} removed from role {_role?.Name}");
	}

	/// <inheritdoc />
	void IProcessor<RemoveUserFromRoleRequest>.NotifyBeforeHookFailure()
	{
		Notifier.Error("Unable to remove user from role");
	}

	/// <inheritdoc />
	void IProcessor<RemoveUserFromRoleRequest>.NotifyProcessFailure()
	{
		Notifier.Error("An unknown error occurred while removing user from role");
	}

	/// <inheritdoc />
	void IProcessor<RemoveUserFromRoleRequest>.NotifyAfterHookFailure()
	{
		Notifier.Warning($"User {_user?.Username} removed from role {_role?.Name} successfully, but a third party plugin failed to execute");
	}

#endregion

	private Task<SienarUser?> GetSienarUserWithRoles(Guid id)
		=> EntitySet
			.Include(u => u.Roles)
			.FirstOrDefaultAsync(u => u.Id == id);
}