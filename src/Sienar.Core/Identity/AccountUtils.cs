using System;
using System.Threading.Tasks;
using Sienar.Configuration;
using Sienar.Email;

namespace Sienar.Identity;

public static class AccountUtils
{
	/// <summary>
	/// Sends a user their welcome email
	/// </summary>
	/// <param name="self">the <see cref="SienarUser"/> to whom the welcome email should be sent</param>
	/// <param name="codeManager">the verification code manager</param>
	/// <param name="emailManager">the account emailer</param>
	public static async Task SendWelcomeEmail(
		this SienarUser self,
		IVerificationCodeManager codeManager,
		IAccountEmailManager emailManager)
	{
		var code = await codeManager.CreateCode(
			self,
			VerificationCodeTypes.Email);

		await emailManager.SendWelcomeEmail(
			self.Username,
			self.Email,
			self.Id,
			code.Code);
	}

	/// <summary>
	/// Sends a user an email change confirmation email 
	/// </summary>
	/// <param name="self">the <see cref="SienarUser"/> to whom the confirmation email should be sent</param>
	/// <param name="vcManager">the verification code manager</param>
	/// <param name="emailManager">the account emailer</param>
	/// <exception cref="InvalidOperationException">if the user doesn't have a pending email change to confirm</exception>
	public static async Task SendEmailChangeConfirmationEmail(
		this SienarUser self,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager)
	{
		if (string.IsNullOrEmpty(self.PendingEmail))
		{
			throw new InvalidOperationException($"Cannot send email change confirmation email: user {self.Id} has no pending email.");
		}

		var code = await vcManager.CreateCode(
			self,
			VerificationCodeTypes.ChangeEmail);

		await emailManager.SendEmailChangeConfirmationEmail(
			self.Username,
			self.PendingEmail,
			self.Id,
			code.Code);
	}

	public static async Task SendPasswordResetEmail(
		this SienarUser user,
		IVerificationCodeManager vcManager,
		IAccountEmailManager emailManager)
	{
		var code = await vcManager.CreateCode(
			user,
			VerificationCodeTypes.PasswordReset);

		await emailManager.SendPasswordResetEmail(
			user.Username,
			user.Email,
			user.Id,
			code.Code);
	}

	public static bool ShouldSendEmailConfirmationEmail(
		LoginOptions loginOptions,
		SienarOptions sienarOptions)
		=> loginOptions.RequireConfirmedAccount && sienarOptions.EnableEmail;
}