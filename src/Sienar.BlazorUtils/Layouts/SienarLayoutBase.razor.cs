using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Menus;
using Sienar.State;

namespace Sienar.Layouts;

public abstract partial class SienarLayoutBase : IDisposable
{
	private bool _disposed;
	private List<List<MenuLink>> _menus = [];
	protected IEnumerable<string> MenuNames = Array.Empty<string>();

	[Inject]
	private ThemeState ThemeState { get; set; } = default!;

	[Inject]
	private IMenuGenerator MenuGenerator { get; set; } = default!;

	[Inject]
	private AuthenticationStateProvider AuthState { get; set; } = default!;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		base.OnInitialized();

		AuthState.AuthenticationStateChanged += UpdateMenuAndRender;
		UpdateMenuAndRender(AuthState.GetAuthenticationStateAsync());
	}

	private void UpdateMenuAndRender(Task<AuthenticationState> s)
	{
		_menus.Clear();
		_menus = MenuNames
			.Select(menuName => MenuGenerator.CreateMenu(menuName))
			.ToList();
		StateHasChanged();
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (_disposed) return;
		if (!disposing) return;

		AuthState.AuthenticationStateChanged -= UpdateMenuAndRender;
		_disposed = true;
	}
}