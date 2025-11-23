using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Sienar.Data;

namespace Sienar.Identity;

[EntityName(Singular = "user", Plural = "users")]
public class SienarUser : EntityBase
{
#region Security

	/// <summary>
	/// The user's username
	/// </summary>
	[PersonalData]
	public string Username { get; set; } = default!;

	/// <summary>
	/// The user's normalized username
	/// </summary>
	[JsonIgnore]
	public string NormalizedUsername { get; set; } = string.Empty;

	/// <summary>
	/// The user's salted and hashed password
	/// </summary>
	[JsonIgnore]
	public string PasswordHash { get; set; } = string.Empty;

	/// <summary>
	/// The user's plain text password
	/// </summary>
	/// <remarks>
	/// This property is only used to update a user's password. The value is never stored or logged.
	/// </remarks>
	[Required]
	[DataType(DataType.Password)]
	[StringLength(64, ErrorMessage = "Password must be between 8 and 64 characters", MinimumLength = 8)]
	public string Password { get; set; } = SienarConstants.PasswordPlaceholder;

	/// <summary>
	/// A confirmation copy of the user's plain text password
	/// </summary>
	/// <remarks>
	/// This property is only used to update a user's password. The value is never stored or logged.
	/// </remarks>
	[Required]
	[DataType(DataType.Password)]
	[Compare("Password", ErrorMessage = "The passwords do not match")]
	public string ConfirmPassword { get; set; } = SienarConstants.PasswordPlaceholder;

	/// <summary>
	/// The number of failed login attempts
	/// </summary>
	public int LoginFailedCount { get; set; }

	/// <summary>
	/// The end date of the lockout period
	/// </summary>
	public DateTime? LockoutEnd { get; set; }

	/// <summary>
	/// A list of verification codes
	/// </summary>
	[JsonIgnore]
	public List<VerificationCode> VerificationCodes { get; set; } = [];

	public List<SienarRole> Roles { get; set; } = [];

	public List<LockoutReason> LockoutReasons { get; set; } = [];

#endregion

#region Contact information

	/// <summary>
	/// The user's email address
	/// </summary>
	[PersonalData]
	[EmailAddress]
	public string Email { get; set; } = string.Empty;

	/// <summary>
	/// The user's normalized email address
	/// </summary>
	[JsonIgnore]
	public string NormalizedEmail { get; set; } = string.Empty;

	/// <summary>
	/// Whether the email address for the user has been confirmed
	/// </summary>
	public bool EmailConfirmed { get; set; }

	/// <summary>
	/// The user's pending email address
	/// </summary>
	[PersonalData]
	public string? PendingEmail { get; set; }

	/// <summary>
	/// The user's normalized pending email address
	/// </summary>
	[JsonIgnore]
	public string? NormalizedPendingEmail { get; set; }

#endregion

	/// <inheritdoc/>
	public override string ToString() => Username;
}
