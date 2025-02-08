using Sienar.Configuration;
using Microsoft.AspNetCore.Components;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

public partial class Menu
{
	/// <summary>
	/// The HTML element with which to render the menu
	/// </summary>
	[Parameter]
	public string Tag { get; set; } = MenuDefaults.Tag;

	/// <summary>
	/// The child content to render
	/// </summary>
	[Parameter]
	public required RenderFragment ChildContent { get; set; }
}
