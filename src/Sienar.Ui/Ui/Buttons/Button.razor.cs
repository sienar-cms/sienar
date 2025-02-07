using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sienar.Configuration;
using Sienar.Extensions;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

/// <summary>
/// A Bulma-styled HTML <c>&lt;button&gt;</c> element
/// </summary>
public partial class Button
{
	private string _tag = "button";

	/// <summary>
	/// The theme color of the button
	/// </summary>
	[Parameter]
	public Color? Color { get; set; } = ButtonDefaults.Color;

	/// <summary>
	/// The size of the button
	/// </summary>
	[Parameter]
	public Size? Size { get; set; } = ButtonDefaults.Size;

	/// <summary>
	/// Whether the button should be responsive
	/// </summary>
	[Parameter]
	public bool Responsive { get; set; } = ButtonDefaults.Responsive;

	/// <summary>
	/// Whether the button should be full-width
	/// </summary>
	[Parameter]
	public bool FullWidth { get; set; } = ButtonDefaults.FullWidth;

	/// <summary>
	/// Whether the button should be light
	/// </summary>
	[Parameter]
	public bool Light { get; set; } = ButtonDefaults.Light;

	/// <summary>
	/// Whether the button should be dark
	/// </summary>
	[Parameter]
	public bool Dark { get; set; } = ButtonDefaults.Dark;

	/// <summary>
	/// Whether the button should be outlined
	/// </summary>
	[Parameter]
	public bool Outlined { get; set; } = ButtonDefaults.Outlined;

	/// <summary>
	/// Whether the button is in a loading state
	/// </summary>
	[Parameter]
	public bool Loading { get; set; }

	/// <summary>
	/// The child content
	/// </summary>
	[Parameter]
	public required RenderFragment? ChildContent { get; set; }

	[CascadingParameter]
	private Dropdown? Dropdown { get; set; }

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		if (UserAttributes is null) return;

		if (UserAttributes.ContainsKey("href"))
		{
			_tag = "a";
		} else if (UserAttributes.ContainsKey("type"))
		{
			_tag = "input";
		}
	}

	private void HandleClicked(MouseEventArgs e)
	{
		if (UserAttributes is not null)
		{
			if (UserAttributes.TryGetValue("onclick", out var onclick))
			{
				((EventCallback<MouseEventArgs>)onclick).InvokeAsync(e);
			}
		}

		Dropdown?.CloseMenu();
	}

	private string CreateCssClasses()
	{
		var classes = $"button is-{Color.GetHtmlValue()}";

		if (Light) classes += " is-light";
		else if (Dark) classes += " is-dark";

		if (Size.HasValue) classes += $" is-{Size.GetHtmlValue()}";
		if (Responsive) classes += " is-responsive";
		if (FullWidth) classes += " is-fullwidth";
		if (Outlined) classes += " is-outlined";
		if (Loading) classes += " is-loading";

		if (Dropdown is not null) classes += " dropdown-item";

		return MergeCssClasses(classes);
	}
}
