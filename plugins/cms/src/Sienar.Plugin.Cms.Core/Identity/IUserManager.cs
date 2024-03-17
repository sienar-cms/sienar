using System;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sienar.Identity;

public interface IUserManager
{
	/// <summary>
	/// Gets a user from a <see cref="ClaimsPrincipal"/>
	/// </summary>
	/// <param name="claims">The <see cref="ClaimsPrincipal"/> for the current user</param>
	/// <returns>the user if it exists, else null</returns>
	Task<SienarUser?> GetSienarUser(ClaimsPrincipal claims);

	/// <summary>
	/// Gets a user from a <see cref="ClaimsPrincipal"/>
	/// </summary>
	/// <param name="claims">The <see cref="ClaimsPrincipal"/> for the current user</param>
	/// <param name="include">An expression to pass to the <c>IQueryable&lt;SienarUser&gt;.Include()</c> method</param>
	/// <returns>the user if it exists, else null</returns>
	Task<SienarUser?> GetSienarUser<TProperty>(
		ClaimsPrincipal claims,
		Expression<Func<SienarUser, TProperty>> include);

	/// <summary>
	/// Gets a user by primary key
	/// </summary>
	/// <param name="id">The user ID</param>
	/// <returns>the user if it exists, else null</returns>
	Task<SienarUser?> GetSienarUser(Guid id);

	/// <summary>
	/// Gets a user by primary key
	/// </summary>
	/// <param name="id">The user ID</param>
	/// <param name="include">An expression to pass to the <c>IQueryable&lt;SienarUser&gt;.Include()</c> method</param>
	/// <returns>the user if it exists, else null</returns>
	Task<SienarUser?> GetSienarUser<TProperty>(
		Guid id,
		Expression<Func<SienarUser, TProperty>> include);

	/// <summary>
	/// Gets a user by either username or email address
	/// </summary>
	/// <param name="name">The username or email address to search for</param>
	/// <returns>the user if it exists, else null</returns>
	Task<SienarUser?> GetSienarUser(string name);

	/// <summary>
	/// Gets a user by either username or email address
	/// </summary>
	/// <param name="name">The username or email address to search for</param>
	/// <param name="include">An expression to pass to the <c>IQueryable&lt;SienarUser&gt;.Include()</c> method</param>
	/// <returns>the user if it exists, else null</returns>
	Task<SienarUser?> GetSienarUser<TProperty>(
		string name,
		Expression<Func<SienarUser, TProperty>> include);

	/// <summary>
	/// Sets a new password for a user
	/// </summary>
	/// <param name="user">The user for whom to update the password</param>
	/// <param name="newPassword">The new password to set</param>
	Task UpdatePassword(
		SienarUser user,
		string newPassword);

	/// <summary>
	/// Verifies that the user's password is correct
	/// </summary>
	/// <param name="user">The user whose password to verify</param>
	/// <param name="password">The password to test</param>
	/// <returns></returns>
	Task<bool> VerifyPassword(SienarUser user, string password);

	/// <summary>
	/// Deletes the currently logged in user
	/// </summary>
	/// <param name="claims">The <see cref="ClaimsPrincipal"/> whose account to delete</param>
	Task DeleteUser(ClaimsPrincipal claims);
}