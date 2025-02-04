using Sienar.Infrastructure;

namespace Sienar.Ui;

/// <summary>
/// Represents horizontal alignment values
/// </summary>
public enum Alignment
{
	/// <summary>
	/// Horizontally left-aligned. Since this is the default alignment in Bulma, it generally does not apply a CSS class except in specific circumstances
	/// </summary>
	[HtmlValue("")]
	Left,

	/// <summary>
	/// Horizontally centered alignment
	/// </summary>
	[HtmlValue("centered")]
	Centered,

	/// <summary>
	/// Horizontally right-aligned
	/// </summary>
	[HtmlValue("right")]
	Right
}
