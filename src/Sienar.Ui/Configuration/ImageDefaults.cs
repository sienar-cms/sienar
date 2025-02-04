using Sienar.Ui;

namespace Sienar.Configuration;

/// <summary>
/// Contains configurable default parameter values for images
/// </summary>
public static class ImageDefaults
{
	/// <summary>
	/// The default image size
	/// </summary>
	public static ImageSize? ImageSize { get; set; } = null;

	/// <summary>
	/// Whether images should be rounded by default
	/// </summary>
	public static bool Rounded { get; set; }
}
