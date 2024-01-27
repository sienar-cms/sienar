using System;
using System.Threading.Tasks;

namespace Sienar.Identity;

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
		Guid userId,
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
		Guid userId,
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
		Guid userId,
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
		Guid userId,
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
		Guid userId,
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
		Guid userId,
		Guid code);
}