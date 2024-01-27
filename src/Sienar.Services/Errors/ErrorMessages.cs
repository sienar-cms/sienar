using System;

namespace Sienar.Errors;

public static class ErrorMessages
{
	public static class Account
	{
		public const string RegistrationDisabled = "Registration is currently disabled.";
		public const string MustAcceptTos = "You must accept the Terms of Service to create an account.";
		public const string UsernameTaken = "That username is already taken.";
		public const string EmailTaken = "That email address is already taken. Do you need to reset your password instead?";
		public const string LoginRequired = "You must be logged in to perform that action.";
		public const string LoginFailedNotFound = "No account with those credentials was found.";
		public const string LoginFailedInvalid = "Invalid credentials supplied.";
		public const string LoginFailedNotConfirmed = "You have not confirmed your email address. Please check your email for a confirmation link and click it to confirm your email address.";
		public const string LoginFailedNotConfirmedEmailDisabled = "You have not confirmed your email address. We cannot resend your confirmation code because the website administrator has disabled email.";

		public const string VerificationCodeInvalid = "The supplied verification code is invalid.";
		public const string VerificationCodeExpired = "The supplied verification code is expired. A new code has been sent.";
		public const string VerificationCodeExpiredEmailDisabled = "The supplied verification code is expired, and the website administrator has disabled email. A new code cannot be sent.";

		public const string AccountErrorWrongId = "The User ID supplied does not match your account.";
		public const string AccountErrorInvalidId = "No account was found with that User ID.";

		public const string NotFound = "Unable to find user with supplied ID.";
		public const string AccountAlreadyInRole = "The specified user is already in the specified role.";
		public const string AccountNotInRole = "The specified user is not in the specified role.";

		/// <summary>
		/// Returns an error message indicating the lockout status of a user's account
		/// </summary>
		/// <param name="offset">The </param>
		/// <returns></returns>
		public static string GetLockoutMessage(DateTimeOffset? offset)
		{
			return offset == DateTimeOffset.MaxValue
				       ? "Your account is permanently locked."
				       : $"Your account is currently locked. The lock will end on {offset:D} at {offset:h:mm:ss tt}.";
		}
	}

	public static class Roles
	{
		public const string NotFound = "The specified role was not found.";
	}

	public static class States
	{
		public const string DuplicateName = "A State with that name already exists.";
		public const string DuplicateAbbreviation = "A State with that abbreviation already exists.";
	}

	public static class Generic
	{
		public const string Unknown = "An unknown error has occurred.";
		public const string NotFound = "The requested resource was not found.";
		public const string NotLoggedIn = "You must be logged in to perform that action.";
		public const string NoPermission = "You do not have permission to perform that action.";
		public const string Unprocessable = "Your request could not be processed with the provided data. Please check your data and try again.";
		public const string DataConflict = "The data you entered is not valid. Please check your data and try again.";
		public const string DataConcurrencyConflict = "The database record you requested has been updated since you last accessed it. Please reload the page and try again.";
		public const string UnhandledByServiceLayer = "An exception was thrown that was unhandled by the service layer.";
	}

	public const string Database = "There was a database error.";
	public const string ContactIt = "Please contact the IT team if your problem persists.";
}