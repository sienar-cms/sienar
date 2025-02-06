using Sienar.Ui;

namespace Sienar.Configuration;

/// <summary>
/// Contains configurable default parameter values for tab groups
/// </summary>
public static class TabGroupDefaults
{
	/// <summary>
	/// The default HTML tag with which to render the tab group
	/// </summary>
	public static string Tag { get; set; } = "article";

	/// <summary>
	/// The default HTML tag with which to render the tab group content wrapper
	/// </summary>
	public static string ContentTag { get; set; } = "div";

	/// <summary>
	/// The default tab alignment
	/// </summary>
	public static Alignment Alignment { get; set; } = Alignment.Left;

	/// <summary>
	/// The default tab size
	/// </summary>
	public static Size Size { get; set; } = Size.Normal;

	/// <summary>
	/// The default tab style
	/// </summary>
	public static TabStyle TabStyle { get; set; } = TabStyle.Default;

	/// <summary>
	/// The default X-axis padding for the tab body container
	/// </summary>
	public static byte PaddingX { get; set; } = 5;

	/// <summary>
	/// The default Y-axis padding for the tab body container
	/// </summary>
	public static byte PaddingY { get; set; } = 5;

	/// <summary>
	/// Whether tabs should be full width by default
	/// </summary>
	public static bool FullWidth { get; set; }
}
