using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sienar.Configuration;
using Sienar.Extensions;

namespace Sienar.Ui;

/// <summary>
/// A Bulma-styled HTML button wrapper
/// </summary>
public class ButtonGroup : SienarComponentBase
{
	/// <summary>
	/// The HTML tag with which to render the button container
	/// </summary>
	[Parameter]
	public string Tag { get; set; } = ButtonGroupDefaults.Tag;

	/// <summary>
	/// The default size of the contained buttons
	/// </summary>
	/// <remarks>
	/// This value can still be overridden on a specific button by providing a <c>Size</c> parameter to that button.
	/// </remarks>
	[Parameter]
	public Size? Size { get; set; } = ButtonGroupDefaults.Size;

	/// <summary>
	/// The alignment of the contained buttons
	/// </summary>
	[Parameter]
	public Alignment Alignment { get; set; } = ButtonGroupDefaults.Alignment;

	/// <summary>
	/// Whether the button container should render buttons as addons
	/// </summary>
	[Parameter]
	public bool HasAddons { get; set; } = ButtonGroupDefaults.HasAddons;

	/// <summary>
	/// The child content to render
	/// </summary>
	[Parameter]
	public required RenderFragment ChildContent { get; set; }

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenElement(0, Tag);
		builder.AddMultipleAttributes(1, UserAttributes);
		builder.AddAttribute(2, "class", CreateCssClasses());
		builder.AddContent(3, ChildContent);
		builder.CloseElement();
	}

	private string CreateCssClasses()
	{
		var classes = "buttons";

		if (Size.HasValue) classes += $" are-{Size.GetHtmlValue()}";
		if (Alignment > Alignment.Left) classes += $" is-{Alignment.GetHtmlValue()}";
		if (HasAddons) classes += " has-addons";

		return MergeCssClasses(classes);
	}
}
