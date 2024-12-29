using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sienar.Identity;

namespace Sienar.Email;

/// <ignore/>
public class AccountEmailManager : IAccountEmailManager
{
	private readonly EmailSenderOptions _senderOptions;
	private readonly IdentityEmailSubjectOptions _identitySubjectOptions;
	private readonly IVerificationCodeManager _vcManager;
	private readonly IAccountEmailMessageFactory _factory;
	private readonly IEmailSender _sender;

	/// <ignore/>
	public AccountEmailManager(
		IOptions<EmailSenderOptions> options,
		IOptions<IdentityEmailSubjectOptions> identityOptions,
		IVerificationCodeManager vcManager,
		IAccountEmailMessageFactory factory,
		IEmailSender sender)
	{
		_senderOptions = options.Value;
		_identitySubjectOptions = identityOptions.Value;
		_vcManager = vcManager;
		_factory = factory;
		_sender = sender;
	}

	/// <inheritdoc />
	public async Task<bool> SendWelcomeEmail(
		SienarUser user,
		VerificationCode? code = null)
	{
		code ??= await _vcManager.CreateCode(
			user,
			VerificationCodeTypes.Email);

		var message = CreateMessage(
			user,
			_identitySubjectOptions.WelcomeEmail,
			await _factory.WelcomeEmailHtml(user.Username, user.Id, code.Code),
			await _factory.WelcomeEmailText(user.Username, user.Id, code.Code));

		return await _sender.Send(message);
	}

	/// <inheritdoc />
	public async Task<bool> SendEmailChangeConfirmationEmail(
		SienarUser user,
		VerificationCode? code = null)
	{
		if (string.IsNullOrEmpty(user.PendingEmail))
		{
			throw new InvalidOperationException($"Cannot send email change confirmation email: user {user.Id} has no pending email.");
		}

		code ??= await _vcManager.CreateCode(
			user,
			VerificationCodeTypes.ChangeEmail);

		var message = CreateMessage(
			user.PendingEmail,
			user.Username,
			_identitySubjectOptions.EmailChange,
			await _factory.ChangeEmailHtml(user.Username, user.Id, code.Code),
			await _factory.ChangeEmailText(user.Username, user.Id, code.Code));

		return await _sender.Send(message);
	}

	/// <inheritdoc />
	public async Task<bool> SendPasswordResetEmail(
		SienarUser user,
		VerificationCode? code = null)
	{
		code ??= await _vcManager.CreateCode(
			user,
			VerificationCodeTypes.PasswordReset);

		var message = CreateMessage(
			user,
			_identitySubjectOptions.PasswordReset,
			await _factory.ResetPasswordHtml(user.Username, user.Id, code.Code),
			await _factory.ResetPasswordText(user.Username, user.Id, code.Code));

		return await _sender.Send(message);
	}

	/// <inheritdoc />
	public async Task<bool> SendAccountLockedEmail(SienarUser user)
	{
		var message = CreateMessage(
			user,
			_identitySubjectOptions.AccountLocked,
			await _factory.AccountLockedHtml(user.Username, user.LockoutEnd!.Value, user.LockoutReasons),
			await _factory.AccountLockedText(user.Username, user.LockoutEnd!.Value, user.LockoutReasons));

		return await _sender.Send(message);
	}

	/// <summary>
	/// Creates a <see cref="MailMessage"/> using the given message details
	/// </summary>
	/// <param name="user">the recipient's account</param>
	/// <param name="subject">The email subject</param>
	/// <param name="htmlBody">The email's HTML version</param>
	/// <param name="textBody">The email's text version</param>
	/// <returns>the <see cref="MailMessage"/></returns>
	private MailMessage CreateMessage(
		SienarUser user,
		string subject,
		string htmlBody,
		string textBody)
		=> CreateMessage(
			user.Email,
			user.Username,
			subject,
			htmlBody,
			textBody);

	/// <summary>
	/// Creates a <see cref="MailMessage"/> using the given message details
	/// </summary>
	/// <param name="email">the recipient's email address</param>
	/// <param name="name">the recipient's name</param>
	/// <param name="subject">The email subject</param>
	/// <param name="htmlBody">The email's HTML version</param>
	/// <param name="textBody">The email's text version</param>
	/// <returns>the <see cref="MailMessage"/></returns>
	private MailMessage CreateMessage(
		string email,
		string name,
		string subject,
		string htmlBody,
		string textBody)
	{
		var message = new MailMessage();
		message.To.Add(new MailAddress(email, name));
		message.From = new MailAddress(_senderOptions.FromAddress, _senderOptions.FromName);
		message.Subject = subject;
		message.Body = htmlBody;
		message.IsBodyHtml = true;

		var plaintext = AlternateView.CreateAlternateViewFromString(
			textBody,
			new ContentType("text/plain"));
		message.AlternateViews.Add(plaintext);

		return message;
	}
}