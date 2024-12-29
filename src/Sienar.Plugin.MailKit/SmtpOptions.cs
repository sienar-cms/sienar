using MailKit.Security;

namespace Sienar.Email;

public class SmtpOptions
{
	/// <summary>
	/// The host address of the SMTP server via which to route email
	/// </summary>
	public string Host { get; set; } = string.Empty;

	/// <summary>
	/// The port number at which the SMTP server accepts incoming SMTP requests
	/// </summary>
	public int Port { get; set; }

	/// <summary>
	/// The <see cref="SecureSocketOptions">connection protocol</see> to use when sending SMTP requests
	/// </summary>
	public SecureSocketOptions SecureSocketOptions { get; set; } = SecureSocketOptions.StartTls;

	/// <summary>
	/// The SMTP username to send
	/// </summary>
	public string Username { get; set; } = string.Empty;

	/// <summary>
	/// The SMTP password to send
	/// </summary>
	public string Password { get; set; } = string.Empty;
}