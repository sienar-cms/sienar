using System;

namespace Sienar.Identity.Requests;

/// <summary>
/// Represents a request to view an account's lockout reasons
/// </summary>
public class AccountLockoutRequest
{
	/// <summary>
	/// The verification code for the request
	/// </summary>
	public Guid VerificationCode { get; set; }

	/// <summary>
	/// The ID of the user for which to fetch lockout reasons 
	/// </summary>
	public Guid UserId { get; set; }
}