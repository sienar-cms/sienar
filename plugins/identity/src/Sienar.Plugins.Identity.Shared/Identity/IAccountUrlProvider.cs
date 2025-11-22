namespace Sienar.Identity;

public interface IAccountUrlProvider
{
	/// <summary>
	/// The URL where a user can confirm their account
	/// </summary>
	string ConfirmationUrl { get; }

	/// <summary>
	/// The URL where a user can confirm their new email address
	/// </summary>
	string EmailChangeUrl { get; }

	/// <summary>
	/// The URL where a user can confirm their password reset request
	/// </summary>
	string ResetPasswordUrl { get; }
}