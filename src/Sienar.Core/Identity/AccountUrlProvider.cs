using Microsoft.Extensions.Options;
using Sienar.Email;

namespace Sienar.Identity;

public class AccountUrlProvider : IAccountUrlProvider
{
	private readonly EmailOptions _options;

	public AccountUrlProvider(IOptions<EmailOptions> options)
	{
		_options = options.Value;
	}

	/// <inheritdoc />
	public string ConfirmationUrl
		=> $"{_options.ApplicationUrl}/dashboard/account/confirm";

	/// <inheritdoc />
	public string EmailChangeUrl
		=> $"{_options.ApplicationUrl}/dashboard/account/email/confirm";

	/// <inheritdoc />
	public string ResetPasswordUrl
		=> $"{_options.ApplicationUrl}/dashboard/account/reset-password";
}