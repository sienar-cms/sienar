#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Identity.Login;

/// <exclude />
public static class LoginErrors
{
	public const string NotFound = "No account with those credentials was found.";
	public const string Invalid = "Invalid credentials supplied.";
	public const string NotConfirmed = "You have not confirmed your email address. Please check your email for a confirmation link and click it to confirm your email address.";
	public const string NotConfirmedEmailDisabled = "You have not confirmed your email address. We cannot resend your confirmation code because the website administrator has disabled email.";
	public const string Locked = "You have failed to log in too many times.";
}
