using System;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Sienar.Infrastructure.Plugins;
using Sienar.State;

namespace Sienar.Infrastructure;

public static class SienarServerAppBuilderExtensions
{
	/// <summary>
	/// Adds a plugin to the Sienar app and registers its services in the service collection
	/// </summary>
	/// <param name="self">the Sienar app builder</param>
	/// <typeparam name="TPlugin">the type of the plugin</typeparam>
	/// <returns>the Sienar app builder</returns>
	public static SienarServerAppBuilder AddPlugin<TPlugin>(
		this SienarServerAppBuilder self)
		where TPlugin : class, ISienarPlugin
	{
		self.Builder.Services.AddScoped(typeof(ISienarPlugin), typeof(TPlugin));

		if (TPlugin.StartupPlugin is not null)
		{
			var startupPlugin = (Activator.CreateInstance(TPlugin.StartupPlugin) as ISienarServerStartupPlugin)!;
			startupPlugin.SetupDependencies(self.Builder);
			self.StartupPlugins.Add(startupPlugin);
		}

		return self;
	}

	/// <summary>
	/// Adds an <see cref="ISienarServerStartupPlugin"/> to the Sienar app
	/// </summary>
	/// <param name="self">the Sienar app builder</param>
	/// <typeparam name="TPlugin">the type of the plugin to add</typeparam>
	/// <returns>the Sienar app builder</returns>
	public static SienarServerAppBuilder AddStartupPlugin<TPlugin>(
		this SienarServerAppBuilder self)
		where TPlugin : ISienarServerStartupPlugin, new()
		=> AddStartupPlugin(self, new TPlugin());

	/// <summary>
	/// Adds an instance of <see cref="ISienarServerStartupPlugin"/> to the Sienar app
	/// </summary>
	/// <param name="self">the Sienar app builder</param>
	/// <param name="plugin">an instance of the plugin to add</param>
	/// <returns>the Sienar app builder</returns>
	public static SienarServerAppBuilder AddStartupPlugin(
		this SienarServerAppBuilder self,
		ISienarServerStartupPlugin plugin)
	{
		plugin.SetupDependencies(self.Builder);
		self.StartupPlugins.Add(plugin);

		if (plugin.PluginData is not null)
		{
			self.PluginDataProvider.Add(plugin.PluginData);
		}

		return self;
	}

	/// <summary>
	/// Registers a custom <see cref="MudTheme"/> for use in Sienar's <see cref="ThemeState"/>
	/// </summary>
	/// <param name="self">the Sienar app builder</param>
	/// <param name="isDarkMode">whether the theme represents dark mode or not</param>
	/// <typeparam name="TTheme">the type of the theme to register</typeparam>
	/// <returns>the Sienar app builder</returns>
	public static SienarServerAppBuilder ConfigureTheme<TTheme>(
		this SienarServerAppBuilder self,
		bool isDarkMode = false)
		where TTheme : MudTheme, new()
		=> ConfigureTheme(
			self,
			new TTheme(),
			isDarkMode);

	/// <summary>
	/// Registers a custom <see cref="MudTheme"/> for use in Sienar's <see cref="ThemeState"/>
	/// </summary>
	/// <param name="self">the Sienar app builder</param>
	/// <param name="theme">the <see cref="MudTheme"/> to use</param>
	/// <param name="isDarkMode">whether the theme represents dark mode or not</param>
	/// <returns>the Sienar app builder</returns>
	public static SienarServerAppBuilder ConfigureTheme(
		this SienarServerAppBuilder self,
		MudTheme theme,
		bool isDarkMode = false)
	{
		self.Theme = theme;
		self.IsDarkMode = isDarkMode;
		return self;
	}
}