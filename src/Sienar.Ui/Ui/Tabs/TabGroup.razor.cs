using Microsoft.AspNetCore.Components;
using Sienar.Configuration;
using Sienar.Extensions;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

public partial class TabGroup
{
	/// <summary>
	/// The alignment of the tabs
	/// </summary>
	[Parameter]
	public Alignment Alignment { get; set; } = TabGroupDefaults.Alignment;

	/// <summary>
	/// The size of the tabs
	/// </summary>
	[Parameter]
	public Size Size { get; set; } = TabGroupDefaults.Size;

	/// <summary>
	/// The style of the tabs
	/// </summary>
	[Parameter]
	public TabStyle TabStyle { get; set; } = TabGroupDefaults.TabStyle;

	/// <summary>
	/// Whether the tabs should be full width
	/// </summary>
	[Parameter]
	public bool FullWidth { get; set; } = TabGroupDefaults.FullWidth;

	/// <summary>
	/// The child content to render
	/// </summary>
	[Parameter]
	public required RenderFragment ChildContent { get; set; }

	private string CreateCssClasses()
	{
		var classes = $"tabs is-{Size.GetHtmlValue()}";

		if (Alignment > Alignment.Left) classes += $" is-{Alignment.GetHtmlValue()}";
		if (TabStyle > TabStyle.Default) classes += $" {TabStyle.GetHtmlValue()}";
		if (FullWidth) classes += " is-fullwidth";

		return MergeCssClasses(classes);
	}
}