using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Sienar.Ui;

/// <summary>
/// A component used to render an HTML element dynamically
/// </summary>
public class HtmlElement : SienarComponentBase
{
	/// <summary>
	/// The HTML tag to render
	/// </summary>
	[Parameter]
	public required string Tag { get; set; }

	/// <summary>
	/// The child content to render, if any
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenElement(0, Tag);
		builder.AddMultipleAttributes(1, UserAttributes);
		builder.AddContent(2, ChildContent);
		builder.CloseElement();
	}
}
