using System;
using System.ComponentModel.DataAnnotations;
using Sienar.Data;

namespace Sienar.Identity;

[EntityName(Singular = "verification code", Plural = "verification codes")]
public class VerificationCode : EntityBase
{
	/// <summary>
	/// The unique verification code
	/// </summary>
	public Guid Code { get; set; }

	/// <summary>
	/// The type of the verification code
	/// </summary>
	[MaxLength(25)]
	public required string Type { get; set; }

	/// <summary>
	/// The verification code expiration date
	/// </summary>
	public DateTime ExpiresAt { get; set; }
}