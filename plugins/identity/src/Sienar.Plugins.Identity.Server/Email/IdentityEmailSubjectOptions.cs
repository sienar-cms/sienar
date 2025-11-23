namespace Sienar.Email;

/// <summary>
/// Configures account-related email message subjects
/// </summary>
public class IdentityEmailSubjectOptions
{
	/// <summary>
	/// The subject of the user's welcome email
	/// </summary>
	public string WelcomeEmail { get; set; } = "Please confirm your account";

	/// <summary>
	/// The subject of the user's email change verification email
	/// </summary>
	public string EmailChange { get; set; } = "Confirm your new email address";

	/// <summary>
	/// The subject of the user's password reset verification email
	/// </summary>
	public string PasswordReset { get; set; } = "Password reset";

	/// <summary>
	/// The subject of the user's account locked email
	/// </summary>
	public string AccountLocked { get; set; } = "You account has been locked";
}