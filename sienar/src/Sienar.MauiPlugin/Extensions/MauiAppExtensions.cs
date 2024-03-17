using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Extensions;

public static class MauiAppExtensions
{
	/// <summary>
	/// Gets an instance of the specified service and calls the provided <see cref="Action"/> with the service argument
	/// </summary>
	/// <param name="self">the <see cref="MauiApp">MAUI application</see></param>
	/// <param name="configurer">an <see cref="Action{T}">action</see> which configures the specified service</param>
	/// <typeparam name="T">the type of the service</typeparam>
	/// <returns>the <see cref="MauiApp">MAUI application</see></returns>
	public static MauiApp Configure<T>(
		this MauiApp self,
		Action<T> configurer)
		where T : notnull
	{
		var service = self.Services.GetRequiredService<T>();
		configurer(service);
		return self;
	}

	/// <summary>
	/// Configures the <see cref="IMenuProvider">menu provider</see>
	/// </summary>
	/// <param name="self">the <see cref="MauiApp">MAUI application</see></param>
	/// <param name="configurer">an <see cref="Action{IMenuProvider}">action</see> that configures the menu provider</param>
	/// <returns>the <see cref="MauiApp">MAUI application</see></returns>
	public static MauiApp ConfigureMenu(
		this MauiApp self,
		Action<IMenuProvider> configurer)
		=> Configure(self, configurer);

	/// <summary>
	/// Configures the <see cref="IDashboardProvider">dashboard provider</see>
	/// </summary>
	/// <param name="self">the <see cref="MauiApp">MAUI application</see></param>
	/// <param name="configurer">an <see cref="Action{IDashboardProvider}">action</see> that configures the dashboard provider</param>
	/// <returns>the <see cref="MauiApp">MAUI application</see></returns>
	public static MauiApp ConfigureDashboard(
		this MauiApp self,
		Action<IDashboardProvider> configurer)
		=> Configure(self, configurer);

	/// <summary>
	/// Configures the <see cref="IRoutableAssemblyProvider">routable assembly provider</see>
	/// </summary>
	/// <param name="self">the <see cref="MauiApp">MAUI application</see></param>
	/// <param name="configurer">an <see cref="Action{IRoutableAssemblyProvider}">action</see> that configures the routable assembly provider</param>
	/// <returns>the <see cref="MauiApp">MAUI application</see></returns>
	public static MauiApp ConfigureRoutableAssemblies(
		this MauiApp self,
		Action<IRoutableAssemblyProvider> configurer)
		=> Configure(self, configurer);

	/// <summary>
	/// Configures the <see cref="IComponentProvider">component provider</see>
	/// </summary>
	/// <param name="self">the <see cref="MauiApp">MAUI application</see></param>
	/// <param name="configurer">an <see cref="Action{IComponentProvider}">action</see> that configures the component provider</param>
	/// <returns>the <see cref="MauiApp">MAUI application</see></returns>
	public static MauiApp ConfigureComponents(
		this MauiApp self,
		Action<IComponentProvider> configurer)
		=> Configure(self, configurer);

	public static MauiApp ConfigureScripts(
		this MauiApp self,
		Action<IScriptProvider> configurer)
		=> Configure(self, configurer);

	public static MauiApp ConfigureStyles(
		this MauiApp self,
		Action<IStyleProvider> configurer)
		=> Configure(self, configurer);
}