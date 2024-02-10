using System;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Extensions;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Extensions;

public static class SienarClientAppBuilderExtensions
{
	public static SienarClientAppBuilder AddPlugin<TPlugin>(
		this SienarClientAppBuilder self)
		where TPlugin : class, ISienarPlugin
	{
		self.Builder.Services.AddScoped(typeof(ISienarPlugin), typeof(TPlugin));

		if (TPlugin.StartupPlugin is not null)
		{
			var startupPlugin = (Activator.CreateInstance(TPlugin.StartupPlugin) as ISienarClientStartupPlugin)!;
			startupPlugin.SetupDependencies(self.Builder);
			self.StartupPlugins.Add(startupPlugin);
		}

		return self;
	}

	public static SienarClientAppBuilder AddServices(
		this SienarClientAppBuilder self,
		Action<IServiceCollection> action)
	{
		action(self.Builder.Services);
		return self;
	}

	public static SienarClientAppBuilder ConfigureTheme(
		this SienarClientAppBuilder self,
		MudTheme theme,
		bool isDarkMode = false)
	{
		self.Theme = theme;
		self.IsDarkMode = isDarkMode;
		return self;
	}
}