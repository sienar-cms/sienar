#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class UserRoleChangeProcessor : IProcessor<AddUserToRoleRequest, bool>,
	IProcessor<RemoveUserFromRoleRequest, bool>
{
	private readonly DbContext _context;

	public UserRoleChangeProcessor(DbContext context)
	{
		_context = context;
	}

	async Task<OperationResult<bool>> IProcessor<AddUserToRoleRequest, bool>.Process(AddUserToRoleRequest request)
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

		var role = await _context
			.Set<SienarRole>()
			.FindAsync(request.RoleId);
		if (role is null)
		{
			return new(OperationStatus.NotFound, message: CmsErrors.Roles.NotFound);
		}

		user.Roles.Add(role);
		await _context.SaveChangesAsync();

		return new(OperationStatus.Success, true, $"User {user.Username} added to role {role.Name}");
	}

	async Task<OperationResult<bool>> IProcessor<RemoveUserFromRoleRequest, bool>.Process(RemoveUserFromRoleRequest request)
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
		await _context.SaveChangesAsync();

		return new(OperationStatus.Success, true, $"User {user.Username} removed from role {role.Name}");
	}

	private Task<SienarUser?> GetSienarUserWithRoles(Guid id)
		=> _context
			.Set<SienarUser>()
			.Include(u => u.Roles)
			.FirstOrDefaultAsync(u => u.Id == id);
}