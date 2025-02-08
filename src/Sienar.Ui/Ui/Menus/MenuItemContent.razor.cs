using Microsoft.AspNetCore.Components;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

public partial class MenuItemContent
{
	/// <summary>
	/// The display text of the menu item
	/// </summary>
	[Parameter]
	public required string Text { get; set; }

	/// <summary>
	/// The icon to display next to the menu item
	/// </summary>
	[Parameter]
	public string? Icon { get; set; }
}

