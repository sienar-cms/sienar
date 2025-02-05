// ReSharper disable once CheckNamespace

using Sienar.Infrastructure;

namespace Sienar.Ui;

public enum TabStyle
{
	/// <summary>
	/// The default Bulma tab style
	/// </summary>
	[HtmlValue("")]
	Default,

	/// <summary>
	/// Bulma boxed tab style
	/// </summary>
	[HtmlValue("is-boxed")]
	Boxed,

	/// <summary>
	/// Bulma toggle tab style
	/// </summary>
	[HtmlValue("is-toggle")]
	Toggle,

	/// <summary>
	/// Bulma rounded toggle style
	/// </summary>
	[HtmlValue("is-toggle is-toggle-rounded")]
	ToggleRounded
}
