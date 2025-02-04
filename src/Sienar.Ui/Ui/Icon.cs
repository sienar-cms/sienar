using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sienar.Configuration;
using Sienar.Extensions;

namespace Sienar.Ui;

/// <summary>
/// A Bulma-styled HTML icon
/// </summary>
/// <remarks>
/// <para>
/// This component represents a Bulma-styled HTML icon using FontAwesome icons. It accepts parameters for the FontAwesome <see cref="IconStyle"/>, the Bulma <see cref="Size"/>, and the Bulma <see cref="Color"/>, as well as a <c>string</c> argument representing the name of the FontAwesome icon to render (without the <c>"fa-"</c> prefix).
/// </para>
///
/// <para>
/// However, it doesn't represent all possible use cases for icons. For example, the Bulma docs describe <see href="https://bulma.io/documentation/elements/icon/#icon-text">using icons with text</see>. This is not supported directly by this component because this component is only an implementation of the <c>&lt;span class="icon"&gt;</c> portion. Developers are still free to use the icon text technique in Bulma by using this component to mark up the icon itself, then manually creating the rest of the markup.
/// </para>
/// </remarks>
/// <example>Creating a standalone icon:
/// <code>
/// &lt;Icon
///     Name="home"
///     IconStyle="IconStyle.Thin"
///     Size="Size.Large"/&gt;
/// </code>
/// </example>
/// <example>Creating an icon with accompanying text:
/// <code>
/// // YourComponent.razor
/// &lt;span class="icon-text"&gt;
///   &lt;Icon Name="home"/&gt;
///   &lt;span&gt;Home&lt;/span&gt;
/// &lt;/span&gt;
/// </code>
/// </example>
public class Icon : SienarComponentBase
{
	/// <summary>
	/// The icon style
	/// </summary>
	[Parameter]
	public IconStyle IconStyle { get; set; } = IconDefaults.IconStyle;

	/// <summary>
	/// The icon size
	/// </summary>
	[Parameter]
	public Size Size { get; set; } = IconDefaults.Size;

	/// <summary>
	/// The icon color
	/// </summary>
	[Parameter]
	public Color Color { get; set; } = IconDefaults.Color;

	/// <summary>
	/// The name of the icon to use, <b>without</b> the "fa-" prefix
	/// </summary>
	[Parameter]
	public required string Name { get; set; }

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenElement(0, "span");
		builder.AddMultipleAttributes(1, UserAttributes);
		builder.AddAttribute(2, "class", CreateCssClasses());
		builder.AddContent(3, BuildIcon);
		builder.CloseElement();
	}

	private void BuildIcon(RenderTreeBuilder builder)
	{
		builder.OpenElement(0, "i");
		builder.AddAttribute(
			1,
			"class",
			$"fa-{IconStyle.GetHtmlValue()} fa-{Name}");
		builder.CloseElement();
	}

	private string CreateCssClasses()
	{
		var classes = $"icon is-{Size.GetHtmlValue()} has-text-{Color.GetHtmlValue()}";
		return MergeCssClasses(classes);
	}
}
