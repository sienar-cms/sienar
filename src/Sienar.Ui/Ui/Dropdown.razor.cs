using Microsoft.AspNetCore.Components;
using Sienar.Configuration;

namespace Sienar.Ui;

public partial class Dropdown
{
	private bool _open;

	/// <summary>
	/// The title text of the dropdown
	/// </summary>
	[Parameter]
	public string? Title { get; set; }

	/// <summary>
	/// The icon to display
	/// </summary>
	[Parameter]
	public string? Icon { get; set; }

	/// <summary>
	/// The color of the dropdown activator button
	/// </summary>
	[Parameter]
	public Color? Color { get; set; } = DropdownDefaults.Color;

	/// <summary>
	/// Whether the menu should open on hover
	/// </summary>
	[Parameter]
	public bool Hoverable { get; set; } = DropdownDefaults.Hoverable;

	/// <summary>
	/// Whether the menu should align to the right
	/// </summary>
	[Parameter]
	public bool Right { get; set; } = DropdownDefaults.Right;

	/// <summary>
	/// Whether the menu should open upward
	/// </summary>
	[Parameter]
	public bool Dropup { get; set; } = DropdownDefaults.Dropup;

	/// <summary>
	/// The child content to render
	/// </summary>
	[Parameter]
	public required RenderFragment ChildContent { get; set; }

	private string CreateCssClasses()
	{
		var classes = "dropdown";

		if (_open) classes += " is-active";
		if (Hoverable) classes += " is-hoverable";
		if (Right) classes += " is-right";
		if (Dropup) classes += " is-up";

		return MergeCssClasses(classes);
	}

	/// <summary>
	/// Closes the menu if it is open
	/// </summary>
	public void CloseMenu() => _open = false;

	private void ToggleMenu() => _open = !_open;
}