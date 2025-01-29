using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Sienar.Infrastructure;

namespace Sienar.Layouts;

public class NavBase : ComponentBase, IDisposable
{
	private bool _disposed;

	protected List<List<MenuLink>> Menus = [];
	protected bool MenuOpen;

	[Parameter]
	public required IEnumerable<Enum> MenuNames { get; set; }

	[Inject]
	protected IMenuGenerator MenuGenerator { get; set; } = null!;

	[Inject]
	protected AuthenticationStateProvider AuthState { get; set; } = null!;

	[Inject]
	protected NavigationManager NavManager { get; set; } = null!;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		base.OnInitialized();

		NavManager.LocationChanged += OnNavigate;
		AuthState.AuthenticationStateChanged += UpdateMenuAndRender;
		UpdateMenuAndRender(AuthState.GetAuthenticationStateAsync());
	}

	/// <summary>
	/// Toggles the open state of the navigation drawer
	/// </summary>
	protected void ToggleDrawer() => MenuOpen = !MenuOpen;

	private async void UpdateMenuAndRender(Task<AuthenticationState> s)
	{
		await s;
		Menus.Clear();
		foreach (var name in MenuNames)
		{
			Menus.Add(await MenuGenerator.Create(name));
		}
		StateHasChanged();
	}

	private void OnNavigate(object? sender, LocationChangedEventArgs e)
	{
		if (!MenuOpen) return;
		MenuOpen = false;
		StateHasChanged();
	}

	/// <inheritdoc />
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (_disposed || !disposing) return;

		NavManager.LocationChanged -= OnNavigate;
		AuthState.AuthenticationStateChanged -= UpdateMenuAndRender;
		_disposed = true;
	}
}