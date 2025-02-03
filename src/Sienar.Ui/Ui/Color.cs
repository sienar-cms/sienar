using Sienar.Infrastructure;

namespace Sienar.Ui;

/// <summary>
/// Represents Bulma-supported color schemes
/// </summary>
public enum Color
{
	/// <summary>
	/// The app's primary color
	/// </summary>
	[HtmlValue("primary")]
	Primary,

	/// <summary>
	/// The app's secondary color
	/// </summary>
	[HtmlValue("secondary")]
	Secondary,

	/// <summary>
	/// The app's tertiary color
	/// </summary>
	[HtmlValue("tertiary")]
	Tertiary,

	/// <summary>
	/// The app's link color
	/// </summary>
	[HtmlValue("link")]
	Link,

	/// <summary>
	/// The app's success color
	/// </summary>
	[HtmlValue("success")]
	Success,

	/// <summary>
	/// The app's info color
	/// </summary>
	[HtmlValue("info")]
	Info,

	/// <summary>
	/// The app's warning color
	/// </summary>
	[HtmlValue("warning")]
	Warning,

	/// <summary>
	/// The app's danger color
	/// </summary>
	[HtmlValue("danger")]
	Danger,

	/// <summary>
	/// The app's text color
	/// </summary>
	[HtmlValue("text")]
	Text,

	/// <summary>
	/// The app's white color
	/// </summary>
	[HtmlValue("white")]
	White,

	/// <summary>
	/// The app's light color
	/// </summary>
	[HtmlValue("light")]
	Light,

	/// <summary>
	/// The app's black color
	/// </summary>
	[HtmlValue("black")]
	Black,

	/// <summary>
	/// The app's dark color
	/// </summary>
	[HtmlValue("dark")]
	Dark,

	/// <summary>
	/// Indicates that the color used should be the CSS <c>currentcolor</c> value
	/// </summary>
	[HtmlValue("current")]
	Current,

	/// <summary>
	/// Indicates that the color used should be the CSS <c>inherit</c> value
	/// </summary>
	[HtmlValue("inherit")]
	Inherit
}
