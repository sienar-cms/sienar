using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Sienar.Configuration;
using Sienar.Extensions;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

public partial class TabGroup : ITabGroup
{
	/// <inheritdoc />
	public List<ITab> Tabs { get; } = [];

	/// <summary>
	/// The HTML tag with which to render the tab group
	/// </summary>
	[Parameter]
	public string Tag { get; set; } = TabGroupDefaults.Tag;

	/// <summary>
	/// The HTML tag with which to render the tab group content wrapper
	/// </summary>
	[Parameter]
	public string ContentTag { get; set; } = TabGroupDefaults.ContentTag;

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
	/// The X-axis padding for the tab body container
	/// </summary>
	[Parameter]
	public byte PaddingX { get; set; } = TabGroupDefaults.PaddingX;

	/// <summary>
	/// The Y-axis padding for the tab body container
	/// </summary>
	[Parameter]
	public byte PaddingY { get; set; } = TabGroupDefaults.PaddingY;

	/// <summary>
	/// Whether the tabs should be full width
	/// </summary>
	[Parameter]
	public bool FullWidth { get; set; } = TabGroupDefaults.FullWidth;

	/// <summary>
	/// The currently active tab
	/// </summary>
	[Parameter]
	public int Active { get; set; }

	/// <summary>
	/// A callback that is executed when the active tab changes
	/// </summary>
	[Parameter]
	public EventCallback<int> ActiveChanged { get; set; }

	/// <summary>
	/// The child content to render
	/// </summary>
	[Parameter]
	public required RenderFragment ChildContent { get; set; }

	/// <inheritdoc />
	protected override void OnAfterRender(bool first)
	{
		if (first && Tabs.Count > Active)
		{
			Tabs[Active].SetActive();
		} 
	}

	/// <inheritdoc />
	public void AddTab(ITab tab)
	{
		Tabs.Add(tab);
		StateHasChanged();
	}

	/// <inheritdoc />
	public void RemoveTab(ITab tab)
	{
		if (Tabs.Remove(tab)) StateHasChanged();
	}

	private void SetActive(int a)
	{
		Tabs[Active].SetInactive();
		Active = a;
		Tabs[Active].SetActive();

		ActiveChanged.InvokeAsync(a);
	}

	private string CreateCssClasses()
	{
		var classes = $"tabs is-flex-direction-column is-{Size.GetHtmlValue()}";

		if (Alignment > Alignment.Left) classes += $" is-{Alignment.GetHtmlValue()}";
		if (TabStyle > TabStyle.Default) classes += $" {TabStyle.GetHtmlValue()}";
		if (FullWidth) classes += " is-fullwidth";

		return MergeCssClasses(classes);
	}
}