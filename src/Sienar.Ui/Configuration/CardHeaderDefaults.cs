using Sienar.Ui;

namespace Sienar.Configuration;

/// <summary>
/// Contains configurable default parameter values for card headers
/// </summary>
public static class CardHeaderDefaults
{
	/// <summary>
	/// The default card header HTML tag
	/// </summary>
	public static string Tag { get; set; } = "header";

	/// <summary>
	/// The default card title HTML tag
	/// </summary>
	public static string TitleTag { get; set; } = "h1";

	/// <summary>
	/// The default card theme color
	/// </summary>
	public static Color? Color { get; set; } = null;
}
