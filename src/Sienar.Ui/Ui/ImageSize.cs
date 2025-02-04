using Sienar.Infrastructure;

namespace Sienar.Ui;

/// <summary>
/// Represents official Bulma-supported image sizes
/// </summary>
public enum ImageSize
{
	/// <summary>
	/// Represents an image with 16x16 dimensions
	/// </summary>
	[HtmlValue("is-16x16")]
	Square16,

	/// <summary>
	/// Represents an image with 24x24 dimensions
	/// </summary>
	[HtmlValue("is-24x24")]
	Square24,

	/// <summary>
	/// Represents an image with 32x32 dimensions
	/// </summary>
	[HtmlValue("is-32x32")]
	Square32,

	/// <summary>
	/// Represents an image with 48x48 dimensions
	/// </summary>
	[HtmlValue("is-48x48")]
	Square48,

	/// <summary>
	/// Represents an image with 64x64 dimensions
	/// </summary>
	[HtmlValue("is-64x64")]
	Square64,

	/// <summary>
	/// Represents an image with 96x96 dimensions
	/// </summary>
	[HtmlValue("is-96x96")]
	Square96,

	/// <summary>
	/// Represents an image with 128x128 dimensions
	/// </summary>
	[HtmlValue("is-128x128")]
	Square128,

	/// <summary>
	/// Represents an image with a square ratio
	/// </summary>
	[HtmlValue("is-square")]
	RatioSquare,

	/// <summary>
	/// Represents an image with a 1x1 ratio
	/// </summary>
	[HtmlValue("is-1by1")]
	Ratio1x1,

	/// <summary>
	/// Represents an image with a 5x4 ratio
	/// </summary>
	[HtmlValue("is-5by4")]
	Ratio5x4,

	/// <summary>
	/// Represents an image with a 4x3 ratio
	/// </summary>
	[HtmlValue("is-4by3")]
	Ratio4x3,

	/// <summary>
	/// Represents an image with a 3x2 ratio
	/// </summary>
	[HtmlValue("is-3by2")]
	Ratio3x2,

	/// <summary>
	/// Represents an image with a 5x3 ratio
	/// </summary>
	[HtmlValue("is-5by3")]
	Ratio5x3,

	/// <summary>
	/// Represents an image with a 16x9 ratio
	/// </summary>
	[HtmlValue("is-16by9")]
	Ratio16x9,

	/// <summary>
	/// Represents an image with a 2x1 ratio
	/// </summary>
	[HtmlValue("is-2by1")]
	Ratio2x1,

	/// <summary>
	/// Represents an image with a 3x1 ratio
	/// </summary>
	[HtmlValue("is-3by1")]
	Ratio3x1,

	/// <summary>
	/// Represents an image with a 4x5 ratio
	/// </summary>
	[HtmlValue("is-4by5")]
	Ratio4x5,

	/// <summary>
	/// Represents an image with a 3x4 ratio
	/// </summary>
	[HtmlValue("is-3by4")]
	Ratio3x4,

	/// <summary>
	/// Represents an image with a 2x3 ratio
	/// </summary>
	[HtmlValue("is-2by3")]
	Ratio2x3,

	/// <summary>
	/// Represents an image with a 3x5 ratio
	/// </summary>
	[HtmlValue("is-3by5")]
	Ratio3x5,

	/// <summary>
	/// Represents an image with a 9x16 ratio
	/// </summary>
	[HtmlValue("is-9by16")]
	Ratio9x16,

	/// <summary>
	/// Represents an image with a 1x2 ratio
	/// </summary>
	[HtmlValue("is-1by2")]
	Ratio1x2,

	/// <summary>
	/// Represents an image with a 1x3 ratio
	/// </summary>
	[HtmlValue("is-1by3")]
	Ratio1x3
}
