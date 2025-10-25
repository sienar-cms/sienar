using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sienar.Identity;

namespace Sienar.Email;

public interface IAccountEmailMessageFactory
{
	/// <summary>
	/// Creates an HTML version of the new account welcome email
	/// </summary>
	/// <param name="username">The user's username</param>
	/// <param name="userId">The user's ID</param>
	/// <param name="code">The verification code</param>
	/// <returns>the string of the email body</returns>
	Task<string> WelcomeEmailHtml(
		string username,
		int userId,
		Guid code);

	/// <summary>
	/// Creates a text version of the new account welcome email
	/// </summary>
	/// <param name="username">The user's username</param>
	/// <param name="userId">The user's ID</param>
	/// <param name="code">The verification code</param>
	/// <returns>the string of the email body</returns>
	Task<string> WelcomeEmailText(
		string username,
		int userId,
		Guid code);

	/// <summary>
	/// Creates an HTML version of the account email change confirmation
	/// </summary>
	/// <param name="username">The user's username</param>
	/// <param name="userId">The user's ID</param>
	/// <param name="code">The verification code</param>
	/// <returns>the string of the email body</returns>
	Task<string> ChangeEmailHtml(
		string username,
		int userId,
		Guid code);

	/// <summary>
	/// Creates a text version of the account email change confirmation
	/// </summary>
	/// <param name="username">The user's username</param>
	/// <param name="userId">The user's ID</param>
	/// <param name="code">The verification code</param>
	/// <returns>the string of the email body</returns>
	Task<string> ChangeEmailText(
		string username,
		int userId,
		Guid code);

	/// <summary>
	/// Creates an HTML version of the account password reset confirmation
	/// </summary>
	/// <param name="username">The user's username</param>
	/// <param name="userId">The user's ID</param>
	/// <param name="code">The verification code</param>
	/// <returns>the string of the email body</returns>
	Task<string> ResetPasswordHtml(
		string username,
		int userId,
		Guid code);

	/// <summary>
	/// Creates a text version of the account password reset confirmation
	/// </summary>
	/// <param name="username">The user's username</param>
	/// <param name="userId">The user's ID</param>
	/// <param name="code">The verification code</param>
	/// <returns>the string of the email body</returns>
	Task<string> ResetPasswordText(
		string username,
		int userId,
		Guid code);

	/// <summary>
	/// Creates an HTML version of the account locked email
	/// </summary>
	/// <param name="username"></param>
	/// <param name="lockoutEnd"></param>
	/// <param name="lockoutReasons"></param>
	/// <returns></returns>
	Task<string> AccountLockedHtml(
		string username,
		DateTime lockoutEnd,
		List<LockoutReason> lockoutReasons);

	/// <summary>
	/// Creates an HTML version of the account locked email
	/// </summary>
	/// <param name="username"></param>
	/// <param name="lockoutEnd"></param>
	/// <param name="lockoutReasons"></param>
	/// <returns></returns>
	Task<string> AccountLockedText(
		string username,
		DateTime lockoutEnd,
		List<LockoutReason> lockoutReasons);
}