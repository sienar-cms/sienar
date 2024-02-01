namespace Sienar.Email;

public class IdentityEmailOptions
{
	/// <summary>
	/// The subject of the user's welcome email
	/// </summary>
	public string WelcomeEmailSubject { get; set; } = "Welcome to our app!";

	/// <summary>
	/// The subject of the user's email change verification email
	/// </summary>
	public string EmailChangeSubject { get; set; } = "Confirm your new email address";

	/// <summary>
	/// The subject of the user's password reset verification email
	/// </summary>
	public string PasswordResetSubject { get; set; } = "Password reset";
}