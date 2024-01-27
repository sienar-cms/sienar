using System;
using System.Threading.Tasks;

namespace Sienar.Identity;

public interface IUserRoleService
{
	/// <summary>
	/// Adds the specified user to the specified role
	/// </summary>
	/// <param name="userId">The ID of the user to add to the role</param>
	/// <param name="roleId">The ID of the role to which to add the user</param>
	/// <returns>whether the operation was successful</returns>
	Task<bool> AddUserToRole(Guid userId, Guid roleId);

	/// <summary>
	/// Removes the specified user from the specified role
	/// </summary>
	/// <param name="userId">The ID of the user to remove from the role</param>
	/// <param name="roleId">The ID of the role to which to remove the user</param>
	/// <returns>whether the operation was successful</returns>
	Task<bool> RemoveUserFromRole(Guid userId, Guid roleId);
}