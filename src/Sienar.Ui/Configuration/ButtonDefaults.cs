using Sienar.Ui;

namespace Sienar.Configuration;

/// <summary>
/// Contains configurable default parameter values for buttons
/// </summary>
public static class ButtonDefaults
{
	/// <summary>
	/// The default button color
	/// </summary>
	public static Color Color { get; set; } = Color.Primary;

	/// <summary>
	/// The default button size
	/// </summary>
	public static Size? Size { get; set; } = null;

	/// <summary>
	/// Whether buttons should be responsive by default
	/// </summary>
	public static bool Responsive { get; set; } = false;

	/// <summary>
	/// Whether buttons should be full-width by default
	/// </summary>
	public static bool FullWidth { get; set; } = false;

	/// <summary>
	/// Whether buttons should be light by default
	/// </summary>
	public static bool Light { get; set; } = false;

	/// <summary>
	/// Whether buttons should be dark by default
	/// </summary>
	public static bool Dark { get; set; } = false;

	/// <summary>
	/// Whether buttons should be outlined by default
	/// </summary>
	public static bool Outlined { get; set; } = false;
}
