using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sienar.Configuration;
using Sienar.Extensions;

namespace Sienar.Ui;

/// <summary>
/// A Bulma-styled HTML <c>&lt;button&gt;</c> element
/// </summary>
public class Button : SienarComponentBase
{
	private string _tag = "button";

	/// <summary>
	/// The theme color of the button
	/// </summary>
	[Parameter]
	public Color Color { get; set; } = ButtonDefaults.Color;

	/// <summary>
	/// The size of the button
	/// </summary>
	[Parameter]
	public Size? Size { get; set; } = ButtonDefaults.Size;

	/// <summary>
	/// Whether the button should be responsive
	/// </summary>
	[Parameter]
	public bool Responsive { get; set; } = ButtonDefaults.Responsive;

	/// <summary>
	/// Whether the button should be full-width
	/// </summary>
	[Parameter]
	public bool FullWidth { get; set; } = ButtonDefaults.FullWidth;

	/// <summary>
	/// Whether the button should be light
	/// </summary>
	[Parameter]
	public bool Light { get; set; } = ButtonDefaults.Light;

	/// <summary>
	/// Whether the button should be dark
	/// </summary>
	[Parameter]
	public bool Dark { get; set; } = ButtonDefaults.Dark;

	/// <summary>
	/// Whether the button should be outlined
	/// </summary>
	[Parameter]
	public bool Outlined { get; set; } = ButtonDefaults.Outlined;

	/// <summary>
	/// Whether the button is in a loading state
	/// </summary>
	[Parameter]
	public bool Loading { get; set; }

	/// <summary>
	/// The child content
	/// </summary>
	[Parameter]
	public required RenderFragment? ChildContent { get; set; }

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		if (Attributes is null) return;

		if (Attributes.ContainsKey("href"))
		{
			_tag = "a";
		} else if (Attributes.ContainsKey("type"))
		{
			_tag = "input";
		}
	}

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenElement(0, _tag);
		builder.AddMultipleAttributes(1, Attributes);
		builder.AddAttribute(2, "class", CreateCssClasses());
		builder.AddContent(3, ChildContent);
		builder.CloseElement();
	}

	private string CreateCssClasses()
	{
		var classes = $"button is-{Color.GetHtmlValue()}";

		if (Light) classes += " is-light";
		else if (Dark) classes += " is-dark";

		if (Size.HasValue) classes += $" is-{Size.GetHtmlValue()}";
		if (Responsive) classes += " is-responsive";
		if (FullWidth) classes += " is-fullwidth";
		if (Outlined) classes += " is-outlined";
		if (Loading) classes += " is-loading";

		return MergeCssClasses(classes);
	}
}
