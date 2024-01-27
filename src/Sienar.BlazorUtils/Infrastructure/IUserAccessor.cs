using System;
using System.Security.Claims;

namespace Sienar.Infrastructure;

public interface IUserAccessor
{
	/// <summary>
	/// Determines whether the current user is currently logged in
	/// </summary>
	/// <returns>the login status</returns>
	bool IsSignedIn();

	/// <summary>
	/// Gets the GUID of the currently logged in user, if any
	/// </summary>
	/// <returns>the GUID</returns>
	Guid? GetUserId();

	/// <summary>
	/// Gets the username of the currently logged in user, if any
	/// </summary>
	/// <returns>the username</returns>
	string? GetUsername();

	/// <summary>
	/// Gets the <see cref="ClaimsPrincipal"/> of the currently logged in user
	/// </summary>
	/// <returns>the <see cref="ClaimsPrincipal"/></returns>
	ClaimsPrincipal GetUserClaimsPrincipal();

	/// <summary>
	/// Determines whether the currently logged in user is in the given role
	/// </summary>
	/// <param name="roleName"></param>
	/// <returns></returns>
	bool UserInRole(string roleName);
}