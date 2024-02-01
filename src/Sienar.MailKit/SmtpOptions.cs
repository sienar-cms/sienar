using MailKit.Security;

namespace Sienar.Email;

public class SmtpOptions
{
	public string Host { get; set; } = string.Empty;
	public int Port { get; set; }
	public SecureSocketOptions SecureSocketOptions { get; set; } = SecureSocketOptions.StartTls;
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
}