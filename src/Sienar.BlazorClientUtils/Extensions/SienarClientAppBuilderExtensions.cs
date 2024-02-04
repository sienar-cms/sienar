using System;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Extensions;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Extensions;

public static class SienarClientAppBuilderExtensions
{
	public static TBuilder AddPlugin<TBuilder, TPlugin>(
		this TBuilder self,
		TPlugin plugin)
		where TBuilder : SienarClientAppBuilder
		where TPlugin : class, ISienarClientPlugin
	{
		self.Plugins.Add(plugin);
		plugin.SetupDependencies(self.Builder);
		self.Builder.Services.AddSingleton<ISienarClientPlugin>(plugin);
		self.Builder.Services.AddSingleton(plugin);
		plugin.SetupRootComponents(self.RootComponentProvider);

		if (plugin.PluginSettings.HasRoutableComponents)
			self.RoutableAssemblyProvider.Add(plugin.GetType().Assembly);

		return self;
	}

	public static TBuilder AddServices<TBuilder>(
		this TBuilder self,
		Action<IServiceCollection> action)
		where TBuilder : SienarClientAppBuilder
	{
		action(self.Builder.Services);
		return self;
	}

	public static TBuilder ConfigureTheme<TBuilder>(
		this TBuilder self,
		MudTheme theme,
		bool isDarkMode = false)
		where TBuilder : SienarClientAppBuilder
	{
		self.Theme = theme;
		self.IsDarkMode = isDarkMode;
		return self;
	}
}