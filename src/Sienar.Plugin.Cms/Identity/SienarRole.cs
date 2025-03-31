using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sienar.Data;

namespace Sienar.Identity;

[EntityName(Singular = "role", Plural ="roles")]
public class SienarRole : EntityBase
{
	/// <summary>
	/// Represents the name of the role
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Represents the normalized name of the role
	/// </summary>
	[JsonIgnore]
	public string NormalizedName { get; set; } = string.Empty;

	/// <summary>
	/// A list of all users in this role
	/// </summary>
	[JsonIgnore]
	public List<SienarUser> Users { get; set; } = [];

	/// <inheritdoc/>
	public override string ToString() => Name;
}