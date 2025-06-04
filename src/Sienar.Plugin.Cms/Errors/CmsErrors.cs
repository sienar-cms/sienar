#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Errors;

/// <exclude />
public static class CmsErrors
{
	public static class Account
	{
		public const string RegistrationDisabled = "Registration is currently disabled.";
		public const string MustAcceptTos = "You must accept the Terms of Service to create an account.";
		public const string UsernameTaken = "That username is already taken.";
		public const string EmailTaken = "That email address is already taken. Do you need to reset your password instead?";
		public const string LoginRequired = "You must be logged in to perform that action.";
		public const string AccountLocked = "Your account has been locked.";
		public const string AccountDeleted = "Your account has been deleted.";

		public const string VerificationCodeInvalid = "The supplied verification code is invalid.";
		public const string VerificationCodeExpired = "The supplied verification code is expired. A new code has been sent.";
		public const string VerificationCodeExpiredEmailDisabled = "The supplied verification code is expired, and the website administrator has disabled email. A new code cannot be sent.";

		public const string AccountErrorWrongId = "The User ID supplied does not match your account.";
		public const string AccountErrorInvalidId = "No account was found with that User ID.";

		public const string NotFound = "Unable to find user with supplied ID.";
		public const string AccountAlreadyInRole = "The specified user is already in the specified role.";
		public const string AccountNotInRole = "The specified user is not in the specified role.";

		public const string NoPendingEmail = "You do not have a pending email change";
	}

	public static class Roles
	{
		public const string NotFound = "The specified role was not found.";
	}

	public static class Email
	{
		public const string FailedToSend = "Unable to send email";
	}
}