using System;
using System.Threading.Tasks;
using Sienar.Email;
using Sienar.Identity;

namespace Sienar.Extensions;

public static class AccountEmailManagerExtensions
{
	/// <summary>
	/// Sends a user their welcome email
	/// </summary>
	/// <param name="self">the account emailer</param>
	/// <param name="vcManager">the verification code manager</param>
	/// <param name="user">the <see cref="SienarUser"/> to whom the email should be sent</param>
	public static async Task SendWelcomeEmail(
		this IAccountEmailManager self,
		IVerificationCodeManager vcManager,
		SienarUser user)
	{
		var code = await vcManager.CreateCode(
			user,
			VerificationCodeTypes.Email);

		await self.SendWelcomeEmail(
			user.Username,
			user.Email,
			user.Id,
			code.Code);
	}

	/// <summary>
	/// Sends a user an email change confirmation email 
	/// </summary>
	/// <param name="self">the account emailer</param>
	/// <param name="vcManager">the verification code manager</param>
	/// <param name="user">the <see cref="SienarUser"/> to whom the email should be sent</param>
	/// <exception cref="InvalidOperationException">if the user doesn't have a pending email change to confirm</exception>
	public static async Task SendEmailChangeConfirmationEmail(
		this IAccountEmailManager self,
		IVerificationCodeManager vcManager,
		SienarUser user)
	{
		if (string.IsNullOrEmpty(user.PendingEmail))
		{
			throw new InvalidOperationException($"Cannot send email change confirmation email: user {user.Id} has no pending email.");
		}

		var code = await vcManager.CreateCode(
			user,
			VerificationCodeTypes.ChangeEmail);

		await self.SendEmailChangeConfirmationEmail(
			user.Username,
			user.PendingEmail,
			user.Id,
			code.Code);
	}

	/// <summary>
	/// Sends a user a password reset confirmation email 
	/// </summary>
	/// <param name="self">the account emailer</param>
	/// <param name="vcManager">the verification code manager</param>
	/// <param name="user">the <see cref="SienarUser"/> to whom the email should be sent</param>
	public static async Task SendPasswordResetEmail(
		this IAccountEmailManager self,
		IVerificationCodeManager vcManager,
		SienarUser user)
	{
		var code = await vcManager.CreateCode(
			user,
			VerificationCodeTypes.PasswordReset);

		await self.SendPasswordResetEmail(
			user.Username,
			user.Email,
			user.Id,
			code.Code);
	}
}