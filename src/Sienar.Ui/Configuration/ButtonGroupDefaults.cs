using Sienar.Ui;

namespace Sienar.Configuration;

/// <summary>
/// Contains configurable default parameter values for button containers
/// </summary>
public static class ButtonGroupDefaults
{
	/// <summary>
	/// The default button container HTML tag
	/// </summary>
	public static string Tag { get; set; } = "div";

	/// <summary>
	/// The default button size of contained buttons
	/// </summary>
	public static Size? Size { get; set; } = null; 

	/// <summary>
	/// The default button alignment
	/// </summary>
	public static Alignment Alignment { get; set; } = Alignment.Left;

	/// <summary>
	/// Whether button containers should have addons by default
	/// </summary>
	public static bool HasAddons { get; set; }
}
