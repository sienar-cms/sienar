using Sienar.Ui;

namespace Sienar.Configuration;

/// <summary>
/// Contains configurable default parameter values for progress bars
/// </summary>
public static class ProgressBarDefaults
{
	/// <summary>
	/// The default progress bar color
	/// </summary>
	public static Color Color { get; set; } = Color.Primary;

	/// <summary>
	/// the default progress bar size
	/// </summary>
	public static Size Size { get; set; } = Size.Normal;

	/// <summary>
	/// The default max value
	/// </summary>
	public static float Max { get; set; } = 100;
}
