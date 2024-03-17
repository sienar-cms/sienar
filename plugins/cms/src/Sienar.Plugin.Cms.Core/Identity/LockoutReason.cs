using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Sienar.Infrastructure.Entities;

namespace Sienar.Identity;

[EntityName(Singular = "lockout reason", Plural = "lockout reasons")]
[Index(nameof(Reason), IsUnique = true)]
public class LockoutReason : EntityBase
{
	[Required]
	[StringLength(255, MinimumLength = 1)]
	public string Reason { get; set; } = string.Empty;

	public List<SienarUser> Users { get; set; } = [];

	/// <inheritdoc />
	public override string ToString() => Reason;
}