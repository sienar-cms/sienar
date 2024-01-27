using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Sienar.Configuration;
using Message = System.Net.Mail.MailMessage;

namespace Sienar.Email;

public class MailKitSender : IEmailSender
{
	private readonly ISmtpClient _client;
	private readonly SmtpOptions _options;
	private readonly SienarOptions _sienarOptions;
	private readonly ILogger<MailKitSender> _logger;

	public MailKitSender(
		ISmtpClient client,
		IOptions<SmtpOptions> options,
		IOptions<SienarOptions> sienarOptions,
		ILogger<MailKitSender> logger)
	{
		_client = client;
		_options = options.Value;
		_sienarOptions = sienarOptions.Value;
		_logger = logger;
	}

	/// <inheritdoc />
	public async Task<bool> Send(Message message)
	{
		if (!_sienarOptions.EnableEmail)
		{
			throw new InvalidOperationException(
				"A service attempted to send an email message, but email is currently disabled. Check the stack trace to determine where the message was sent and add a conditional that checks if email is enabled prior to sending.");
		}

		_logger.LogInformation(
			"Sending email to SMTP server at {Host}:{Port}. Encryption: {Secure}",
			_options.Host, _options.Port, _options.SecureSocketOptions);

		try
		{
			using var mailkitMessage = (MimeMessage)message;
			await _client.ConnectAsync(
				_options.Host,
				_options.Port,
				_options.SecureSocketOptions);
			if (_options.SecureSocketOptions != SecureSocketOptions.None)
			{
				await _client.AuthenticateAsync(_options.Username, _options.Password);
			}

			await _client.SendAsync(mailkitMessage);
			await _client.DisconnectAsync(true);
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to send message via MailKit");
			return false;
		}
	}
}