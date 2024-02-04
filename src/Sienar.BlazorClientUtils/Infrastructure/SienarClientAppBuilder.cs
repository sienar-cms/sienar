using System.Collections.Generic;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Sienar.Extensions;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;
using Sienar.State;

namespace Sienar.Infrastructure;

public class SienarClientAppBuilder
{
	public readonly WebAssemblyHostBuilder Builder;
	public readonly IList<ISienarClientPlugin> Plugins = [];
	public readonly IRootComponentProvider RootComponentProvider = new RootComponentProvider();
	public readonly IRoutableAssemblyProvider RoutableAssemblyProvider = new RoutableAssemblyProvider();
	public MudTheme? Theme;
	public bool IsDarkMode;

	protected SienarClientAppBuilder(WebAssemblyHostBuilder builder)
	{
		Builder = builder;
	}

	public static SienarClientAppBuilder Create(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);
		builder.Services.AddBlazorWasmUtils();
		return new SienarClientAppBuilder(builder);
	}

	public virtual WebAssemblyHost Build()
	{
		// Set up remaining services on the IServiceCollection
		Theme ??= new MudTheme();
		var themeState = new ThemeState
		{
			Theme = Theme,
			IsDarkMode = IsDarkMode
		};
		Builder.Services.AddSingleton(themeState);
		Builder.Services.AddSingleton(RoutableAssemblyProvider);

		foreach (var selector in RootComponentProvider.RootComponents)
		{
			Builder.RootComponents.Add(selector.Value, selector.Key);
		}

		var app = Builder.Build();

		var menuProvider = app.Services.GetRequiredKeyedService<IMenuProvider>(
			SienarBlazorUtilsServiceKeys.MenuProvider);
		var dashboardProvider = app.Services.GetRequiredKeyedService<IMenuProvider>(
			SienarBlazorUtilsServiceKeys.DashboardProvider);
		var componentProvider = app.Services.GetRequiredService<IComponentProvider>();

		foreach (var plugin in Plugins)
		{
			plugin.SetupApp(app);
			plugin.SetupMenu(menuProvider);
			plugin.SetupDashboard(dashboardProvider);
			plugin.SetupComponents(componentProvider);
		}

		return app;
	}
}