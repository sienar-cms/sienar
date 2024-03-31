using System;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using MudBlazor.Services;
using Sienar.Extensions;
using Sienar.Infrastructure.Hooks;
using Sienar.Layouts;

namespace Sienar.Infrastructure.Plugins;

public class SienarMauiBlazorPlugin : IDesktopPlugin
{
	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "Sienar MAUI Blazor Plugin",
		Version = Version.Parse("0.1.0"),
		Author = "Christian LeVesque",
		AuthorUrl = "https://levesque.dev",
		Description = "Sienar MAUI Plugin provides all of the services and configuration required to operate a MAUI Blazor app using the Sienar framework. Sienar cannot function without this plugin.",
		Homepage = "https://sienar.levesque.dev"
	};

	/// <inheritdoc />
	public void SetupDependencies(MauiAppBuilder builder)
	{
		builder.Services
			.AddCascadingAuthenticationState()
			.AddAuthorizationCore()
			.AddSienarCoreUtilities(isDesktop: true)
			.AddSingleton<AuthenticationStateProvider, AuthStateProvider>()
			.AddTransient<IUserAccessor, MauiUserAccessor>()
			.AddTransient<INotificationService, NotificationService>()
			.AddTransient(typeof(IStateValidator<>), typeof(ConcurrencyStampValidator<>))
			.AddTransient(typeof(IBeforeProcess<>), typeof(ConcurrencyStampUpdateHook<>))
			.AddMudServices();
	}

	/// <inheritdoc />
	public void SetupApp(MauiApp app)
	{
		app.ConfigureComponents(c => c.DefaultLayout ??= typeof(DashboardLayout));
	}
}