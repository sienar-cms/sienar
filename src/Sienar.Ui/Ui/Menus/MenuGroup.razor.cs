using Microsoft.AspNetCore.Components;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

public partial class MenuGroup
{
	private bool _open;

	/// <summary>
	/// The title of the menu group
	/// </summary>
	[Parameter]
	public required string Title { get; set; }

	/// <summary>
	/// The icon to display next to the menu group title
	/// </summary>
	[Parameter]
	public string? Icon { get; set; }

	/// <summary>
	/// The child content to render
	/// </summary>
	[Parameter]
	public required RenderFragment ChildContent { get; set; }

	private void Toggle() => _open = !_open;

	private string? CreateAnchorCssClasses()
	{
		return _open ? "is-active" : null;
	}

	private string CreateListCssClasses()
	{
		var classes = "menu-list";
		if (!_open) classes += " is-hidden";
		return classes;
	}
}

