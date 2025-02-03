using Sienar.Infrastructure;

namespace Sienar.Ui;

/// <summary>
/// Represents FontAwesome icon styles
/// </summary>
public enum IconStyle
{
	/// <summary>
	/// A solid icon style (i.e., "fa-solid")
	/// </summary>
	[HtmlValue("solid")]
	Solid,

	/// <summary>
	/// A regular icon style (i.e., "fa-regular")
	/// </summary>
	[HtmlValue("regular")]
	Regular,

	/// <summary>
	/// A light icon style (i.e., "fa-light")
	/// </summary>
	[HtmlValue("light")]
	Light,

	/// <summary>
	/// A thin icon style (i.e., "fa-thin")
	/// </summary>
	[HtmlValue("thin")]
	Thin,

	/// <summary>
	/// A brand icon style (i.e., "fa-brands")
	/// </summary>
	[HtmlValue("brands")]
	Brands
}
