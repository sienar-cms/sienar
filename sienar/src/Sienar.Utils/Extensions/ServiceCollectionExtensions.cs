using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;
using Sienar.Infrastructure.Services;

namespace Sienar.Extensions;

public static class ServiceCollectionExtensions
{
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
	/// Checks if a <see cref="TOptions"/> has already been configured, and if not, adds the supplied default configuration
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="config">the default configuration to apply if no existing configuration was found</param>
	/// <typeparam name="TOptions">the type of the options class to configure</typeparam>
	/// <returns></returns>
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

	public static TService? GetAndRemoveService<TService>(
		this IServiceCollection self)
		=> (TService?)GetAndRemoveService(self, typeof(TService));

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

	public static void RemoveService<TService>(this IServiceCollection self)
		=> RemoveService(self, typeof(TService));
}