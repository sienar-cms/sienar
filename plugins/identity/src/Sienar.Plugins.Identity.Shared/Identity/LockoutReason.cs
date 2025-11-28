using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sienar.Data;

namespace Sienar.Identity;

/// <summary>
/// A reason why a user might be locked out
/// </summary>
[EntityName(Singular = "lockout reason", Plural = "lockout reasons")]
public class LockoutReason : EntityBase
{
	/// <summary>
	/// The reason why a user might be locked out
	/// </summary>
	public string Reason { get; set; } = string.Empty;

	/// <summary>
	/// The normalized reason why a user might be locked out
	/// </summary>
	[JsonIgnore]
	public string NormalizedReason { get; set; } = string.Empty;

	/// <summary>
	/// The users who are locked out for this reason
	/// </summary>
	[JsonIgnore]
	public List<SienarUser> Users { get; set; } = [];

	/// <inheritdoc />
	public override string ToString() => Reason;
}