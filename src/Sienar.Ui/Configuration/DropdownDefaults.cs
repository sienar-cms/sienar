using Sienar.Ui;

namespace Sienar.Configuration;

/// <summary>
/// Contains configurable default parameter values for cards
/// </summary>
public static class DropdownDefaults
{
	/// <summary>
	/// The default theme color for dropdown activator buttons
	/// </summary>
	public static Color? Color { get; set; } = null;

	/// <summary>
	/// Whether dropdowns should open on hover by default
	/// </summary>
	public static bool Hoverable { get; set; }

	/// <summary>
	/// Whether dropdowns should align to the right edge by default
	/// </summary>
	public static bool Right { get; set; }

	/// <summary>
	/// Whether dropdowns should open up by default
	/// </summary>
	public static bool Dropup { get; set; }
}
