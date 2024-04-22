using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;
using Sienar.Infrastructure.Services;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="IServiceCollection"/> extension methods for the <c>Sienar.Utils</c> assembly
/// </summary>
public static class SienarUtilsServiceCollectionExtensions
{
	/// <summary>
	/// Adds universal Sienar utilities to the DI container
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="isDesktop">whether the current application is a desktop application (<c>true</c>) or a web application (<c>false</c>)</param>
	/// <returns>the service collection</returns>
	[ExcludeFromCodeCoverage]
	public static IServiceCollection AddSienarCoreUtilities(
		this IServiceCollection self,
		bool isDesktop = false)
	{
		self.TryAddSingleton<IMenuProvider, MenuProvider>();
		self.TryAddSingleton<IDashboardProvider, DashboardProvider>();
		self.TryAddSingleton<IRoutableAssemblyProvider, RoutableAssemblyProvider>();
		self.TryAddSingleton<IComponentProvider, ComponentProvider>();
		self.TryAddSingleton<IStyleProvider, StyleProvider>();
		self.TryAddSingleton<IScriptProvider, ScriptProvider>();

		if (isDesktop)
		{
			self.TryAddSingleton<IMenuGenerator, MenuGenerator>();
			self.TryAddSingleton<IDashboardGenerator, DashboardGenerator>();
		}
		else
		{
			self.TryAddScoped<IMenuGenerator, MenuGenerator>();
			self.TryAddScoped<IDashboardGenerator, DashboardGenerator>();
		}
		
		self.TryAddScoped(typeof(IEntityReader<>), typeof(EntityReader<>));
		self.TryAddScoped(typeof(IEntityWriter<>), typeof(EntityWriter<>));
		self.TryAddScoped(typeof(IEntityDeleter<>), typeof(EntityDeleter<>));
		self.TryAddScoped(typeof(IStatusService<>), typeof(StatusService<>));
		self.TryAddScoped(typeof(IService<,>), typeof(Service<,>));
		self.TryAddScoped(typeof(IResultService<>), typeof(ResultService<>));

		return self;
	}

	/// <summary>
	/// Checks if a <c>TOptions</c> has already been configured, and if not, adds the supplied default configuration
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="config">the default configuration to apply if no existing configuration was found</param>
	/// <typeparam name="TOptions">the type of the options class to configure</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection ApplyDefaultConfiguration<TOptions>(
		this IServiceCollection self,
		IConfiguration config)
		where TOptions : class
	{
		if (!self.Any(sd => sd.ServiceType == typeof(IConfigureOptions<TOptions>)))
		{
			self.Configure<TOptions>(config);
		}

		return self;
	}

	/// <summary>
	/// Removes a service from the service collection and returns its implementation instance
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="serviceType">the <c>Type</c> of the service</param>
	/// <returns>the implementation instance if it exists, else <c>null</c></returns>
	public static object? GetAndRemoveService(
		this IServiceCollection self,
		Type serviceType)
	{
		var service = self.FirstOrDefault(
			s => s.ServiceType == serviceType);
		if (service is not null)
		{
			self.Remove(service);
		}

		return service?.ImplementationInstance;
	}

	/// <summary>
	/// Removes a service from the service collection and returns its implementation instance
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TService">the type of the service</typeparam>
	/// <returns>the implementation instance if it exists, else <c>null</c></returns>
	public static TService? GetAndRemoveService<TService>(
		this IServiceCollection self)
		=> (TService?)GetAndRemoveService(self, typeof(TService));

	/// <summary>
	/// Removes a service from the service collection if it is registered
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="serviceType">the <c>Type</c> of the service to remove</param>
	public static void RemoveService(
		this IServiceCollection self,
		Type serviceType)
	{
		var service = self.FirstOrDefault(
			s => s.ServiceType == serviceType);
		if (service is not null)
		{
			self.Remove(service);
		}
	}

	/// <summary>
	/// Removes a service from the service collection if it is registered
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TService">the type of the service to remove</typeparam>
	public static void RemoveService<TService>(this IServiceCollection self)
		=> RemoveService(self, typeof(TService));
}