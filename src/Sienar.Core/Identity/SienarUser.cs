using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Entities;
using Sienar.Utils;

namespace Sienar.Identity;

[EntityName(Singular = "user", Plural = "users")]
[Index(nameof(Username), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class SienarUser : EntityBase
{
#region Security

	/// <summary>
	/// Gets or sets the username
	/// </summary>
	[PersonalData]
	[StringLength(32, ErrorMessage = "Username must be between 6 and 32 characters", MinimumLength = 6)]
	[Required]
	public string Username { get; set; } = default!;

	/// <summary>
	/// Gets or sets a salted and hashed representation of the password
	/// </summary>
	[MaxLength(100)]
	public string PasswordHash { get; set; } = default!;

	/// <summary>
	/// Gets or sets the user's plain text password
	/// </summary>
	/// <remarks>
	/// This property is only used to update a user's password. The value is never stored or logged.
	/// </remarks>
	[NotMapped]
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
	[NotMapped]
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
	public List<VerificationCode> VerificationCodes { get; set; } = new();

	public List<SienarRole> Roles { get; set; } = new();

#endregion

#region Contact information

	/// <summary>
	/// Gets or sets the email address
	/// </summary>
	[PersonalData]
	[MaxLength(100)]
	[Required]
	[EmailAddress]
	public string Email { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets whether the email address for the user has been confirmed
	/// </summary>
	public bool EmailConfirmed { get; set; }

	/// <summary>
	/// Gets or sets the pending email address
	/// </summary>
	[PersonalData]
	[MaxLength(100)]
	public string? PendingEmail { get; set; }

#endregion

	public List<Medium> Media { get; set; } = new();

	/// <inheritdoc/>
	public override string ToString() => Username;
}
