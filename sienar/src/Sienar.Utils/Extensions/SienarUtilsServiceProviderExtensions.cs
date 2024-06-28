using System;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Extensions;

/// <summary>
/// Provides public extension methods for <see cref="IServiceProvider"/>
/// </summary>
public static class SienarUtilsServiceProviderExtensions
{
	/// <summary>
	/// Gets an instance of the specified service and calls the provided <see cref="Action"/> with the service argument
	/// </summary>
	/// <param name="self">the <see cref="IServiceProvider">service provider</see></param>
	/// <param name="configurer">an <see cref="Action{T}">action</see> which configures the specified service</param>
	/// <typeparam name="T">the type of the service</typeparam>
	/// <returns>the <see cref="IServiceProvider">service provider</see></returns>
	public static IServiceProvider Configure<T>(
		this IServiceProvider self,
		Action<T> configurer)
		where T : notnull
	{
		var service = self.GetRequiredService<T>();
		configurer(service);
		return self;
	}

	/// <summary>
	/// Configures the <see cref="IMenuProvider">menu provider</see>
	/// </summary>
	/// <param name="self">the <see cref="IServiceProvider">service provider</see></param>
	/// <param name="configurer">an <see cref="Action{IMenuProvider}">action</see> that configures the menu provider</param>
	/// <returns>the <see cref="IServiceProvider">service provider</see></returns>
	public static IServiceProvider ConfigureMenu(
		this IServiceProvider self,
		Action<IMenuProvider> configurer)
		=> Configure(self, configurer);

	/// <summary>
	/// Configures the <see cref="IDashboardProvider">dashboard provider</see>
	/// </summary>
	/// <param name="self">the <see cref="IServiceProvider">service provider</see></param>
	/// <param name="configurer">an <see cref="Action{IDashboardProvider}">action</see> that configures the dashboard provider</param>
	/// <returns>the <see cref="IServiceProvider">service provider</see></returns>
	public static IServiceProvider ConfigureDashboard(
		this IServiceProvider self,
		Action<IDashboardProvider> configurer)
		=> Configure(self, configurer);

	/// <summary>
	/// Configures the <see cref="IRoutableAssemblyProvider">routable assembly provider</see>
	/// </summary>
	/// <param name="self">the <see cref="IServiceProvider">service provider</see></param>
	/// <param name="configurer">an <see cref="Action{IRoutableAssemblyProvider}">action</see> that configures the routable assembly provider</param>
	/// <returns>the <see cref="IServiceProvider">service provider</see></returns>
	public static IServiceProvider ConfigureRoutableAssemblies(
		this IServiceProvider self,
		Action<IRoutableAssemblyProvider> configurer)
		=> Configure(self, configurer);

	/// <summary>
	/// Configures the <see cref="IComponentProvider">component provider</see>
	/// </summary>
	/// <param name="self">the <see cref="IServiceProvider">service provider</see></param>
	/// <param name="configurer">an <see cref="Action{IComponentProvider}">action</see> that configures the component provider</param>
	/// <returns></returns>
	public static IServiceProvider ConfigureComponents(
		this IServiceProvider self,
		Action<IComponentProvider> configurer)
		=> Configure(self, configurer);

	/// <summary>
	/// Configures the <see cref="IScriptProvider">script provider</see>
	/// </summary>
	/// <param name="self">the <see cref="IServiceProvider">service provider</see></param>
	/// <param name="configurer">an <see cref="Action{IScriptProvider}">action</see> that configures the script provider</param>
	/// <returns>the <see cref="IServiceProvider">service provider</see></returns>
	public static IServiceProvider ConfigureScripts(
		this IServiceProvider self,
		Action<IScriptProvider> configurer)
		=> Configure(self, configurer);

	/// <summary>
	/// Configures the <see cref="IStyleProvider">style provider</see>
	/// </summary>
	/// <param name="self">the <see cref="IServiceProvider">service provider</see></param>
	/// <param name="configurer">an <see cref="Action{IStyleProvider}">action</see> that configures the style provider</param>
	/// <returns>the <see cref="IServiceProvider">service provider</see></returns>
	public static IServiceProvider ConfigureStyles(
		this IServiceProvider self,
		Action<IStyleProvider> configurer)
		=> Configure(self, configurer);
}