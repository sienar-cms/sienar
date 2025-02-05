using Sienar.Ui;

namespace Sienar.Configuration;

/// <summary>
/// Contains configurable default parameter values for tab groups
/// </summary>
public static class TabGroupDefaults
{
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
	/// Whether tabs should be full width by default
	/// </summary>
	public static bool FullWidth { get; set; }
}
