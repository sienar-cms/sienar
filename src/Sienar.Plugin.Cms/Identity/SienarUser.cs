using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Sienar.Data;
using Sienar.Media;

namespace Sienar.Identity;

[EntityName(Singular = "user", Plural = "users")]
public class SienarUser : EntityBase
{
#region Security

	/// <summary>
	/// Gets or sets the username
	/// </summary>
	[PersonalData]
	public string Username { get; set; } = default!;

	/// <summary>
	/// Gets or sets the normalized username
	/// </summary>
	[JsonIgnore]
	public string NormalizedUsername { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets a salted and hashed representation of the password
	/// </summary>
	[JsonIgnore]
	public string PasswordHash { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the user's plain text password
	/// </summary>
	/// <remarks>
	/// This property is only used to update a user's password. The value is never stored or logged.
	/// </remarks>
	[Required]
	[DataType(DataType.Password)]
	[StringLength(64, ErrorMessage = "Password must be between 8 and 64 characters", MinimumLength = 8)]
	public string Password { get; set; } = SienarConstants.PasswordPlaceholder;

	/// <summary>
	/// Gets or sets a confirmation copy of the user's plain text password
	/// </summary>
	/// <remarks>
	/// This property is only used to update a user's password. The value is never stored or logged.
	/// </remarks>
	[Required]
	[DataType(DataType.Password)]
	[Compare("Password", ErrorMessage = "The passwords do not match")]
	public string ConfirmPassword { get; set; } = SienarConstants.PasswordPlaceholder;

	/// <summary>
	/// Gets or sets the number of failed login attempts
	/// </summary>
	public int LoginFailedCount { get; set; }

	/// <summary>
	/// Gets or sets the end date of the lockout period
	/// </summary>
	public DateTime? LockoutEnd { get; set; }

	/// <summary>
	/// Gets or sets a list of verification codes
	/// </summary>
	[JsonIgnore]
	public List<VerificationCode> VerificationCodes { get; set; } = [];

	public List<SienarRole> Roles { get; set; } = [];

	public List<LockoutReason> LockoutReasons { get; set; } = [];

#endregion

#region Contact information

	/// <summary>
	/// Gets or sets the email address
	/// </summary>
	[PersonalData]
	[EmailAddress]
	public string Email { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the normalized email address
	/// </summary>
	[JsonIgnore]
	public string NormalizedEmail { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets whether the email address for the user has been confirmed
	/// </summary>
	public bool EmailConfirmed { get; set; }

	/// <summary>
	/// Gets or sets the pending email address
	/// </summary>
	[PersonalData]
	public string? PendingEmail { get; set; }

	/// <summary>
	/// Gets or sets the normalized pending email address
	/// </summary>
	[JsonIgnore]
	public string? NormalizedPendingEmail { get; set; }

#endregion

	public List<Upload> Media { get; set; } = [];

	/// <inheritdoc/>
	public override string ToString() => Username;
}
