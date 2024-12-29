using Sienar.Data;

namespace Sienar.Identity;

/// <summary>
/// Contains utilities for creating frequently-used <see cref="Filter"/> patterns
/// </summary>
public static class SienarUserFilterFactory
{
	/// <summary>
	/// Creates a new <see cref="Filter"/> that includes the user's roles in the query
	/// </summary>
	/// <returns>the filter</returns>
	public static Filter WithRoles() => new() { Includes = ["Roles"] };
}