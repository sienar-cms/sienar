using System.Collections.Generic;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Sienar.Extensions;
using Sienar.Infrastructure.Plugins;
using Sienar.State;

namespace Sienar.Infrastructure;

public class SienarClientAppBuilder
{
	public readonly WebAssemblyHostBuilder Builder;
	public readonly IList<ISienarClientStartupPlugin> StartupPlugins = [];
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

		var app = Builder.Build();

		foreach (var plugin in StartupPlugins)
		{
			plugin.SetupApp(app);
		}

		return app;
	}
}