using System;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Sienar.Infrastructure.Plugins;
using Sienar.State;

namespace Sienar.Infrastructure;

public static class SienarWebAppBuilderExtensions
{
	/// <summary>
	/// Adds a plugin to the Sienar app and registers its services in the service collection
	/// </summary>
	/// <param name="self">the Sienar app builder</param>
	/// <param name="plugin">the plugin to add to the Sienar app</param>
	/// <typeparam name="TBuilder">the type of the Sienar app builder</typeparam>
	/// <typeparam name="TPlugin">the type of the plugin</typeparam>
	/// <returns>the Sienar app builder</returns>
	public static TBuilder AddPlugin<TBuilder, TPlugin>(
		this TBuilder self,
		TPlugin plugin)
		where TBuilder : SienarServerAppBuilder
		where TPlugin : class, ISienarServerPlugin
	{
		self.Plugins.Add(plugin);
		plugin.SetupDependencies(self.Builder);
		self.Builder.Services.AddSingleton<ISienarServerPlugin>(plugin);
		self.Builder.Services.AddSingleton(plugin);

		return self;
	}

	/// <summary>
	/// Allows developers to add arbitrary services to the service collection without creating a plugin
	/// </summary>
	/// <param name="self">the Sienar app builder</param>
	/// <param name="action">an action that receives the service collection as its only argument</param>
	/// <typeparam name="TBuilder">the type of the Sienar app builder</typeparam>
	/// <returns>the Sienar app builder</returns>
	public static TBuilder AddServices<TBuilder>(
		this TBuilder self,
		Action<IServiceCollection> action)
		where TBuilder : SienarServerAppBuilder
	{
		action(self.Builder.Services);
		return self;
	}

	/// <summary>
	/// Registers a custom <see cref="MudTheme"/> for use in Sienar's <see cref="ThemeState"/>
	/// </summary>
	/// <param name="self">the Sienar app builder</param>
	/// <param name="theme">the <see cref="MudTheme"/> to use</param>
	/// <param name="isDarkMode">whether the theme represents dark mode or not</param>
	/// <typeparam name="TBuilder">the type of the Sienar app builder</typeparam>
	/// <returns>the Sienar app builder</returns>
	public static TBuilder ConfigureTheme<TBuilder>(
		this TBuilder self,
		MudTheme theme,
		bool isDarkMode = false)
		where TBuilder : SienarServerAppBuilder
	{
		self.Theme = theme;
		self.IsDarkMode = isDarkMode;
		return self;
	}
}