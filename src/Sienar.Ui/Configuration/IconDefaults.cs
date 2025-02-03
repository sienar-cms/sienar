using Sienar.Ui;

namespace Sienar.Configuration;

/// <summary>
/// Contains configurable default parameter values for icons
/// </summary>
public static class IconDefaults
{
	/// <summary>
	/// The default icon style
	/// </summary>
	public static IconStyle IconStyle { get; set; } = IconStyle.Solid;

	/// <summary>
	/// The default icon size
	/// </summary>
	public static Size Size { get; set; } = Size.Normal;

	/// <summary>
	/// The default icon color
	/// </summary>
	public static Color Color { get; set; } = Color.Inherit;
}
