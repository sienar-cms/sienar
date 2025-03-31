using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sienar.Data;

namespace Sienar.Identity;

[EntityName(Singular = "lockout reason", Plural = "lockout reasons")]
public class LockoutReason : EntityBase
{
	public string Reason { get; set; } = string.Empty;

	[JsonIgnore]
	public string NormalizedReason { get; set; } = string.Empty;

	[JsonIgnore]
	public List<SienarUser> Users { get; set; } = [];

	/// <inheritdoc />
	public override string ToString() => Reason;
}