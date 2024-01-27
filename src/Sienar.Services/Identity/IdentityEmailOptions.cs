namespace Sienar.Identity;

public class IdentityEmailOptions
{
	public string WelcomeEmailSubject { get; set; } = "Welcome to our app!";
	public string EmailChangeSubject { get; set; } = "Confirm your new email address";
	public string PasswordResetSubject { get; set; } = "Password reset";
}