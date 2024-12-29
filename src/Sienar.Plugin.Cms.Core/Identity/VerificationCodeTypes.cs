namespace Sienar.Identity;

/// <summary>
/// A group of constants that identify various types of verification codes
/// </summary>
/// <remarks>
///	KEEP IN MIND: The database column these are saved to is limited to 25 characters
/// </remarks>
public static class VerificationCodeTypes
{
	public const string Email = "email";
	public const string ChangeEmail = "change-email";
	public const string PasswordReset = "password-reset";
	public const string ViewLockoutReasons = "view-lockout-reasons";
}