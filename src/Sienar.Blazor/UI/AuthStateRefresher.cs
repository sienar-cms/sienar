using System;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Sienar.Identity;
using Sienar.Infrastructure;

namespace Sienar.UI;

public class AuthStateRefresher : ComponentBase, IDisposable
{
	private const int Interval = 60 * 10 * 1000; // 10 minutes, in ms
	private Timer? _timer;
	private bool _disposed;

	[Inject]
	private IBlazorServerSignInManager SignInManager { get; set; } = default!;

	[Inject]
	private ILogger<AuthStateRefresher> Logger { get; set; } = default!;

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			SetupRefreshTimer();
			await SignInManager.LoadUserLoginStatus();
		}
	}

	private void SetupRefreshTimer()
	{
		Logger.LogInformation("Updating refresh timer");
		_timer?.Dispose();
		_timer = new Timer(Interval);
		_timer.Elapsed += RefreshUserLogin;
		_timer.Enabled = true;
		_timer.Start();
	}

	private async void RefreshUserLogin(object? sender, EventArgs e)
	{
		await InvokeAsync(
			async () =>
			{
				Logger.LogInformation("Refreshing user login");
				await SignInManager.RefreshUserLoginStatus();
				SetupRefreshTimer();
			});
	}

	/// <inheritdoc />
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (_disposed)
		{
			return;
		}

		if (disposing)
		{
			if (_timer is not null)
			{
				_timer.Elapsed -= RefreshUserLogin;
				_timer.Dispose();
			}
		}

		_disposed = true;
	}
}