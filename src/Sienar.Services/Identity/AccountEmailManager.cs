using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sienar.Email;

namespace Sienar.Identity;

public class AccountEmailManager : IAccountEmailManager
{
	protected readonly EmailOptions Options;
	protected readonly IdentityEmailOptions IdentityOptions;
	protected readonly IAccountEmailMessageFactory Factory;
	protected readonly IEmailSender Sender;

	public AccountEmailManager(
		IOptions<EmailOptions> options,
		IOptions<IdentityEmailOptions> identityOptions,
		IAccountEmailMessageFactory factory,
		IEmailSender sender)
	{
		Options = options.Value;
		IdentityOptions = identityOptions.Value;
		Factory = factory;
		Sender = sender;
	}

	/// <inheritdoc />
	public virtual async Task<bool> SendWelcomeEmail(
		string username,
		string email,
		Guid userId,
		Guid code)
	{
		var message = CreateMessage(
			username,
			email,
			IdentityOptions.WelcomeEmailSubject,
			await Factory.WelcomeEmailHtml(username, userId, code),
			await Factory.WelcomeEmailText(username, userId, code));

		return await Sender.Send(message);
	}

	/// <inheritdoc />
	public virtual async Task<bool> SendEmailChangeConfirmationEmail(
		string username,
		string email,
		Guid userId,
		Guid code)
	{
		var message = CreateMessage(
			username,
			email,
			IdentityOptions.EmailChangeSubject,
			await Factory.ChangeEmailHtml(username, userId, code),
			await Factory.ChangeEmailText(username, userId, code));

		return await Sender.Send(message);
	}

	/// <inheritdoc />
	public virtual async Task<bool> SendPasswordResetEmail(
		string username,
		string email,
		Guid userId,
		Guid code)
	{
		var message = CreateMessage(
			username,
			email,
			IdentityOptions.PasswordResetSubject,
			await Factory.ResetPasswordHtml(username, userId, code),
			await Factory.ResetPasswordText(username, userId, code));

		return await Sender.Send(message);
	}

	/// <summary>
	/// Creates a <see cref="MailMessage"/> using the given message details
	/// </summary>
	/// <param name="displayName">The recipient's name</param>
	/// <param name="email">The recipient's email address</param>
	/// <param name="subject">The email subject</param>
	/// <param name="htmlBody">The email's HTML version</param>
	/// <param name="textBody">The email's text version</param>
	/// <returns>the <see cref="MailMessage"/></returns>
	protected virtual MailMessage CreateMessage(
		string displayName,
		string email,
		string subject,
		string htmlBody,
		string textBody)
	{
		var message = new MailMessage();
		message.To.Add(new MailAddress(email, displayName));
		message.From = new MailAddress(Options.FromAddress, Options.FromName);
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