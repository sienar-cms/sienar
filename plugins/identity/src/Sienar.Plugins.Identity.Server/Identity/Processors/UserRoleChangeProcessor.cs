#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sienar.Errors;
using Sienar.Identity.Requests;
using Sienar.Infrastructure;
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
			return new(OperationStatus.NotFound, message: CoreErrors.Account.NotFound);
		}

		if (user.Roles.Any(r => r.Id == request.RoleId))
		{
			return new(OperationStatus.Unprocessable, message: CoreErrors.Account.AccountAlreadyInRole);
		}

		var role = await _context
			.Set<SienarRole>()
			.FindAsync(request.RoleId);
		if (role is null)
		{
			return new(OperationStatus.NotFound, message: CoreErrors.Roles.NotFound);
		}

		user.Roles.Add(role);
		_context
			.Set<SienarUser>()
			.Update(user);
		await _context.SaveChangesAsync();
		return new(OperationStatus.Success, true, $"User {user.Username} added to role {role.Name}");
	}

	async Task<OperationResult<bool>> IStatusProcessor<RemoveUserFromRoleRequest>.Process(RemoveUserFromRoleRequest request)
	{
		var user = await GetSienarUserWithRoles(request.UserId);
		if (user is null)
		{
			return new(OperationStatus.NotFound, message: CoreErrors.Account.NotFound);
		}

		var role = user.Roles.FirstOrDefault(r => r.Id == request.RoleId);
		if (role is null)
		{
			return new(OperationStatus.Unprocessable, message: CoreErrors.Account.AccountNotInRole);
		}

		user.Roles.Remove(role);
		_context
			.Set<SienarUser>()
			.Update(user);
		await _context.SaveChangesAsync();

		return new(OperationStatus.Success, true, $"User {user.Username} removed from role {role.Name}");
	}

	private Task<SienarUser?> GetSienarUserWithRoles(int id)
		=> _context
			.Set<SienarUser>()
			.Include(u => u.Roles)
			.FirstOrDefaultAsync(u => u.Id == id);
}