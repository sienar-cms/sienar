using System.Net.Mail;
using System.Threading.Tasks;

namespace Sienar.Email;

public class DefaultEmailSender : IEmailSender
{
	/// <inheritdoc/>
	/// <remarks>
	/// This implementation simply returns <c>true</c>, indicating that an email was sent. However, no email will actually be sent by the default mail sender. This is to avoid requiring developers to adopt a particular mail strategy. Developers who want their app to send email should add their own implementation of <see cref="IEmailSender"/>, or else install one of the existing packages for sending email from Sienar
	/// </remarks>
	public virtual Task<bool> Send(MailMessage message)
	{
		return Task.FromResult(true);
	}
}