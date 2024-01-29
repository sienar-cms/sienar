using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sienar.Email;

namespace Sienar.Identity;

public class AccountEmailManager : IAccountEmailManager
{
	private readonly EmailOptions _options;
	private readonly IdentityEmailOptions _identityOptions;
	private readonly IAccountEmailMessageFactory _factory;
	private readonly IEmailSender _sender;

	public AccountEmailManager(
		IOptions<EmailOptions> options,
		IOptions<IdentityEmailOptions> identityOptions,
		IAccountEmailMessageFactory factory,
		IEmailSender sender)
	{
		_options = options.Value;
		_identityOptions = identityOptions.Value;
		_factory = factory;
		_sender = sender;
	}

	/// <inheritdoc />
	public async Task<bool> SendWelcomeEmail(
		string username,
		string email,
		Guid userId,
		Guid code)
	{
		var message = CreateMessage(
			username,
			email,
			_identityOptions.WelcomeEmailSubject,
			await _factory.WelcomeEmailHtml(username, userId, code),
			await _factory.WelcomeEmailText(username, userId, code));

		return await _sender.Send(message);
	}

	/// <inheritdoc />
	public async Task<bool> SendEmailChangeConfirmationEmail(
		string username,
		string email,
		Guid userId,
		Guid code)
	{
		var message = CreateMessage(
			username,
			email,
			_identityOptions.EmailChangeSubject,
			await _factory.ChangeEmailHtml(username, userId, code),
			await _factory.ChangeEmailText(username, userId, code));

		return await _sender.Send(message);
	}

	/// <inheritdoc />
	public async Task<bool> SendPasswordResetEmail(
		string username,
		string email,
		Guid userId,
		Guid code)
	{
		var message = CreateMessage(
			username,
			email,
			_identityOptions.PasswordResetSubject,
			await _factory.ResetPasswordHtml(username, userId, code),
			await _factory.ResetPasswordText(username, userId, code));

		return await _sender.Send(message);
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
	private MailMessage CreateMessage(
		string displayName,
		string email,
		string subject,
		string htmlBody,
		string textBody)
	{
		var message = new MailMessage();
		message.To.Add(new MailAddress(email, displayName));
		message.From = new MailAddress(_options.FromAddress, _options.FromName);
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