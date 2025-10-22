using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Sienar.Data;

namespace Sienar.Identity.Data;

/// <summary>
/// Adds additional methods for querying <see cref="SienarUser"/> database entries
/// </summary>
public interface IUserRepository : IRepository<SienarUser>
{
	/// <summary>
	/// Gets a user by either username or email address
	/// </summary>
	/// <param name="name">The username or email address to search for</param>
	/// <returns>the user if it exists, else null</returns>
	Task<SienarUser?> ReadUserByNameOrEmail(
		string name,
		Filter? filter = null);

	/// <summary>
	/// Determines whether a username has been taken
	/// </summary>
	/// <param name="id">the ID of the current user, used to determine if the username is taken by someone else</param>
	/// <param name="username">the username to check</param>
	/// <returns><c>true</c> if the username is already taken, else <c>false</c></returns>
	Task<bool> UsernameIsTaken(
		Guid id,
		string username);

	/// <summary>
	/// Determines whether an email address has been taken
	/// </summary>
	/// <param name="id">the ID of the current user, used to determine if the email address is taken by someone else</param>
	/// <param name="email"></param>
	/// <returns></returns>
	Task<bool> EmailIsTaken(
		Guid id,
		string email);

	/// <summary>
	/// Loads a user's <see cref="VerificationCode"/> records
	/// </summary>
	/// <param name="user">The user for which to load verification codes</param>
	Task LoadVerificationCodes(SienarUser user);
}