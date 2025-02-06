using System;
using Microsoft.AspNetCore.Components;
using Sienar.Configuration;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

public sealed partial class Tab : IDisposable
{
	private bool _lastActive;
	private bool _active;

	/// <summary>
	/// The HTML element with which to render a tab
	/// </summary>
	[Parameter]
	public string Tag { get; set; } = TabDefaults.Tag;

	/// <summary>
	/// The tab title to display
	/// </summary>
	[Parameter]
	public string? Title { get; set; }

	/// <summary>
	/// The tab icon to display
	/// </summary>
	[Parameter]
	public string? Icon { get; set; }

	/// <summary>
	/// The child content to render
	/// </summary>
	[Parameter]
	public required RenderFragment ChildContent { get; set; }

	[CascadingParameter]
	private TabGroup? TabGroup { get; set; }

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		TabGroup?.AddTab(this);
	}

	/// <summary>
	/// Informs a tab that it is currently active and should render itself
	/// </summary>
	public void SetActive()
	{
		_active = true;
		if (_active != _lastActive)
		{
			_lastActive = true;
			StateHasChanged();
		}
	}

	/// <summary>
	/// Informs a tab that it is currently inactive and should not render itself
	/// </summary>
	public void SetInactive()
	{
		_active = false;
		if (_active != _lastActive)
		{
			_lastActive = false;
			StateHasChanged();
		}
	}

	/// <inheritdoc />
	public void Dispose()
	{
		TabGroup?.RemoveTab(this);
	}

	private string CreateCssClasses()
	{
		return _active ? "is-block" : "is-hidden";
	}
}