using System.Threading.Tasks;
using Sienar.Identity;

namespace Sienar.Email;

/// <summary>
/// Manages the sending of account-related emails
/// </summary>
public interface IAccountEmailManager
{
	/// <summary>
	/// Sends a user an email to confirm their new account
	/// </summary>
	/// <param name="user">the user who should receive the welcome email</param>
	/// <param name="code">the verification code to send. If not provided, it will be generated</param>
	/// <returns>whether the operation was successful</returns>
	Task<bool> SendWelcomeEmail(
		SienarUser user,
		VerificationCode? code = null);

	/// <summary>
	/// Sends a user an email to confirm their new email address
	/// </summary>
	/// <param name="user">the user who should receive the email change confirmation email</param>
	/// <param name="code">the verification code to send. If not provided, it will be generated</param>
	/// <returns>whether the operation was successful</returns>
	Task<bool> SendEmailChangeConfirmationEmail(
		SienarUser user,
		VerificationCode? code = null);

	/// <summary>
	/// Sends a user an email to start the password recovery process
	/// </summary>
	/// <param name="user">the user who should receive the password reset email</param>
	/// <param name="code">the verification code to send. If not provided, it will be generated</param>
	/// <returns>whether the operation was successful</returns>
	Task<bool> SendPasswordResetEmail(
		SienarUser user,
		VerificationCode? code = null);

	/// <summary>
	/// Sends a user an email notifying them that their account has been locked
	/// </summary>
	/// <param name="user">the user who should receive the account locked email</param>
	/// <returns>whether the operation was successful</returns>
	Task<bool> SendAccountLockedEmail(SienarUser user);
}