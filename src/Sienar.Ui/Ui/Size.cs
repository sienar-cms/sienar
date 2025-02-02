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
	[HtmlValue("is-small")]
	Small,

	/// <summary>
	/// A component with a normal size (default)
	/// </summary>
	[HtmlValue("is-normal")]
	Normal,

	/// <summary>
	/// A component with a medium size
	/// </summary>
	[HtmlValue("is-medium")]
	Medium,

	/// <summary>
	/// A component with a large size
	/// </summary>
	[HtmlValue("is-large")]
	Large
}
