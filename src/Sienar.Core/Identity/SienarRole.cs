using Sienar.Infrastructure.Entities;

namespace Sienar.Identity;

[EntityName(Singular = "role", Plural ="roles")]
public class SienarRole : EntityBase
{
	/// <summary>
	/// Represents the name of the role
	/// </summary>
	public required string Name { get; set; }

	/// <inheritdoc/>
	public override string ToString() => Name;
}