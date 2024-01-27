#nullable disable
using MailKit.Security;

namespace Sienar.Email;

public class SmtpOptions
{
	public string Host { get; set; }
	public int Port { get; set; }
	public SecureSocketOptions SecureSocketOptions { get; set; } = SecureSocketOptions.StartTls;
	public string Username { get; set; }
	public string Password { get; set; }
}