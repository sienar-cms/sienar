using Sienar.Infrastructure;

namespace Sienar.Ui;

/// <summary>
/// Represents the available sizes for Bulma-based components which support different sizes
/// </summary>
public enum Size
{
	/// <summary>
	/// A component with a small size
	/// </summary>
	[HtmlValue("small")]
	Small,

	/// <summary>
	/// A component with a normal size (default)
	/// </summary>
	[HtmlValue("normal")]
	Normal,

	/// <summary>
	/// A component with a medium size
	/// </summary>
	[HtmlValue("medium")]
	Medium,

	/// <summary>
	/// A component with a large size
	/// </summary>
	[HtmlValue("large")]
	Large
}
