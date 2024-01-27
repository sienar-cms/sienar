using System;
using System.Threading.Tasks;

namespace Sienar.Identity;

public interface IAccountEmailManager
{
	/// <summary>
	/// Sends a user an email to confirm their new account
	/// </summary>
	/// <param name="username">The user's username</param>
	/// <param name="email">The user's email address</param>
	/// <param name="userId">The user's ID</param>
	/// <param name="verificationCode">The verification code that confirms the user account</param>
	/// <returns>whether the operation was successful</returns>
	Task<bool> SendWelcomeEmail(
		string username,
		string email,
		Guid userId,
		Guid verificationCode);

	/// <summary>
	/// Sends a user an email to confirm their new email address
	/// </summary>
	/// <param name="username">The user's username</param>
	/// <param name="email">The user's email address</param>
	/// <param name="userId">The user's ID</param>
	/// <param name="verificationCode">The verification code that confirms the user's new email address</param>
	/// <returns>whether the operation was successful</returns>
	Task<bool> SendEmailChangeConfirmationEmail(
		string username,
		string email,
		Guid userId,
		Guid verificationCode);

	/// <summary>
	/// Sends a user an email to start the password recovery process
	/// </summary>
	/// <param name="username">The user's username</param>
	/// <param name="email">The user's email address</param>
	/// <param name="userId">The user's ID</param>
	/// <param name="verificationCode">The verification code that confirms the user's identity</param>
	/// <returns>whether the operation was successful</returns>
	Task<bool> SendPasswordResetEmail(
		string username,
		string email,
		Guid userId,
		Guid verificationCode);
}