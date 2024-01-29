using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Errors;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Services;

namespace Sienar.Identity;

public class UserRoleService
	: DbService<SienarUser>, IUserRoleService
{
	/// <inheritdoc />
	public UserRoleService(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<SienarUser, DbContext>> logger,
		INotificationService notifier)
		: base(contextAccessor, logger, notifier) {}

	/// <inheritdoc />
	public async Task<bool> AddUserToRole(Guid userId, Guid roleId)
	{
		var user = await GetSienarUserWithRoles(userId);
		if (user is null)
		{
			Notifier.Error(ErrorMessages.Account.NotFound);
			return false;
		}

		if (user.Roles.Any(r => r.Id == roleId))
		{
			Notifier.Warning(ErrorMessages.Account.AccountAlreadyInRole);
			return false;
		}

		var role = await Context
			.Set<SienarRole>()
			.FindAsync(roleId);
		if (role is null)
		{
			Notifier.Error(ErrorMessages.Roles.NotFound);
			return false;
		}

		user.Roles.Add(role);
		await Context.SaveChangesAsync();

		Notifier.Success($"User {user.Username} added to role {role.Name}");
		return true;
	}

	/// <inheritdoc />
	public async Task<bool> RemoveUserFromRole(Guid userId, Guid roleId)
	{
		var user = await GetSienarUserWithRoles(userId);
		if (user is null)
		{
			Notifier.Error(ErrorMessages.Account.NotFound);
			return false;
		}

		var roleToRemove = user.Roles.FirstOrDefault(r => r.Id == roleId);
		if (roleToRemove is null)
		{
			Notifier.Warning(ErrorMessages.Account.AccountNotInRole);
			return false;
		}

		user.Roles.Remove(roleToRemove);
		await Context.SaveChangesAsync();

		Notifier.Success($"User {user.Username} removed from role {roleToRemove.Name}");
		return true;
	}

	private Task<SienarUser?> GetSienarUserWithRoles(Guid id)
		=> EntitySet
			.Include(u => u.Roles)
			.FirstOrDefaultAsync(u => u.Id == id);
}