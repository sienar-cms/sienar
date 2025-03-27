using System;
using Sienar.Services;

namespace Sienar.Identity.Results;

/// <summary>
/// Represents the result of a login operation
/// </summary>
public class LoginResult : IResult
{
	/// <summary>
	/// The ID of the user who failed to log in
	/// </summary>
	public Guid UserId { get; set; }

	/// <summary>
	/// The verification code the user can use to view the reason(s) their account is locked
	/// </summary>
	public Guid VerificationCode { get; set; }
}