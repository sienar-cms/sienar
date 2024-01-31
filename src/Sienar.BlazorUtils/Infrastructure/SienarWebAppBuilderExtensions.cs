using System;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Infrastructure;

public static class SienarWebAppBuilderExtensions
{
	public static TBuilder AddPlugin<TBuilder, TPlugin>(
		this TBuilder self,
		TPlugin plugin)
		where TBuilder : SienarWebAppBuilder
		where TPlugin : class, ISienarPlugin
	{
		self.Plugins.Add(plugin);
		plugin.SetupDependencies(self.Builder);
		self.Builder.Services.AddSingleton<ISienarPlugin>(plugin);
		self.Builder.Services.AddSingleton(plugin);

		if (plugin.PluginSettings.ModifiesScripts
			|| plugin.PluginSettings.ModifiesStyles
			|| plugin.PluginSettings.ModifiesMenus
			|| plugin.PluginSettings.HasRoutableComponents)
		{
			self.MiddlewareSetupFuncs.Add(app => app.UsePluginMiddleware<TPlugin>());
		}

		return self;
	}

	public static TBuilder AddServices<TBuilder>(
		this TBuilder self,
		Action<IServiceCollection> action)
		where TBuilder : SienarWebAppBuilder
	{
		action(self.Builder.Services);
		return self;
	}

	public static TBuilder ConfigureTheme<TBuilder>(
		this TBuilder self,
		MudTheme theme,
		bool isDarkMode = false)
		where TBuilder : SienarWebAppBuilder
	{
		self.Theme = theme;
		self.IsDarkMode = isDarkMode;
		return self;
	}
}