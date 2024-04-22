#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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

/// <exclude />
public class UserRoleChangeProcessor : DbService<SienarUser>,
	IProcessor<AddUserToRoleRequest, bool>,
	IProcessor<RemoveUserFromRoleRequest, bool>
{
	public UserRoleChangeProcessor(
		DbContext context,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier)
		: base(context, logger, notifier) {}

	async Task<HookResult<bool>> IProcessor<AddUserToRoleRequest, bool>.Process(AddUserToRoleRequest request)
	{
		var user = await GetSienarUserWithRoles(request.UserId);
		if (user is null)
		{
			return new(HookStatus.NotFound, message: CmsErrors.Account.NotFound);
		}

		if (user.Roles.Any(r => r.Id == request.RoleId))
		{
			return new(HookStatus.Unprocessable, message: CmsErrors.Account.AccountAlreadyInRole);
		}

		var role = await Context
			.Set<SienarRole>()
			.FindAsync(request.RoleId);
		if (role is null)
		{
			return new(HookStatus.NotFound, message: CmsErrors.Roles.NotFound);
		}

		user.Roles.Add(role);
		await Context.SaveChangesAsync();

		return new(HookStatus.Success, true, $"User {user.Username} added to role {role.Name}");
	}

	async Task<HookResult<bool>> IProcessor<RemoveUserFromRoleRequest, bool>.Process(RemoveUserFromRoleRequest request)
	{
		var user = await GetSienarUserWithRoles(request.UserId);
		if (user is null)
		{
			return new(HookStatus.NotFound, message: CmsErrors.Account.NotFound);
		}

		var role = user.Roles.FirstOrDefault(r => r.Id == request.RoleId);
		if (role is null)
		{
			return new(HookStatus.Unprocessable, message: CmsErrors.Account.AccountNotInRole);
		}

		user.Roles.Remove(role);
		await Context.SaveChangesAsync();

		return new(HookStatus.Success, true, $"User {user.Username} removed from role {role.Name}");
	}

	private Task<SienarUser?> GetSienarUserWithRoles(Guid id)
		=> EntitySet
			.Include(u => u.Roles)
			.FirstOrDefaultAsync(u => u.Id == id);
}