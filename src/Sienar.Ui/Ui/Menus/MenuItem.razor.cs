using Microsoft.AspNetCore.Components;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

public partial class MenuItem
{
	private bool _isLink;
	private string _classes = "is-flex is-align-items-center";
	private string? _lastHref;

	/// <summary>
	/// The display text of the menu item
	/// </summary>
	[Parameter]
	public required string Text { get; set; }

	/// <summary>
	/// The URL to which the menu item points
	/// </summary>
	[Parameter]
	public string? Href { get; set; }

	/// <summary>
	/// The icon to display next to the menu item
	/// </summary>
	[Parameter]
	public string? Icon { get; set; }

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		if (!string.IsNullOrEmpty(Href)) _isLink = true;
	}
}

