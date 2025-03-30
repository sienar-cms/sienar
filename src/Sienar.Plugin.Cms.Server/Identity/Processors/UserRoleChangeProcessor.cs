#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Data;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class UserRoleChangeProcessor<TContext>
	: IStatusProcessor<AddUserToRoleRequest>,
		IStatusProcessor<RemoveUserFromRoleRequest>
	where TContext : DbContext
{
	private readonly TContext _context;

	public UserRoleChangeProcessor(TContext context)
	{
		_context = context;
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

		var role = await _context.FindAsync<SienarRole>(request.RoleId);
		if (role is null)
		{
			return new(OperationStatus.NotFound, message: CmsErrors.Roles.NotFound);
		}

		user.Roles.Add(role);
		_context.Update(user);
		await _context.SaveChangesAsync();

		return new(
				OperationStatus.Success,
				true,
				$"User {user.Username} added to role {role.Name}");
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
		_context.Update(user);
		await _context.SaveChangesAsync();

		return new(OperationStatus.Success, true, $"User {user.Username} removed from role {role.Name}");
	}

	private Task<SienarUser?> GetSienarUserWithRoles(Guid id)
		=> _context
			.Set<SienarUser>()
			.Where(u => u.Id == id)
			.Include(u => u.Roles)
			.FirstOrDefaultAsync();
}