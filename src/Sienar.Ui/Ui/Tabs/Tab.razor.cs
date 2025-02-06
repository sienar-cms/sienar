using System;
using Microsoft.AspNetCore.Components;
using Sienar.Configuration;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

public sealed partial class Tab : ITab, IDisposable
{
	private bool _lastActive;
	private bool _active;

	/// <summary>
	/// The HTML element with which to render a tab
	/// </summary>
	[Parameter]
	public string Tag { get; set; } = TabDefaults.Tag;

	/// <inheritdoc />
	[Parameter]
	public string? Title { get; set; }

	/// <inheritdoc />
	[Parameter]
	public string? Icon { get; set; }

	/// <summary>
	/// The child content to render
	/// </summary>
	[Parameter]
	public required RenderFragment ChildContent { get; set; }

	[CascadingParameter]
	private ITabGroup? TabGroup { get; set; }

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		TabGroup?.AddTab(this);
	}

	/// <inheritdoc />
	public void SetActive()
	{
		_active = true;
		if (_active != _lastActive)
		{
			_lastActive = true;
			StateHasChanged();
		}
	}

	/// <inheritdoc />
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