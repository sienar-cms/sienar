using System.Threading.Tasks;

namespace Sienar.Identity;

public interface IPasswordManager
{
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
}