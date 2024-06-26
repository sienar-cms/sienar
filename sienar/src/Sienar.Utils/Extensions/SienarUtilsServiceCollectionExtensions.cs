using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;
using Sienar.Infrastructure.Processors;
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

	/// <summary>
	/// Replaces a service in the service collection with a different implementation if the registered service type matches the given implementation
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="serviceType">the type of the service</param>
	/// <param name="oldImplementationType">the type of the implementation to replace</param>
	/// <param name="newImplementationType">the type of the new implementation</param>
	public static IServiceCollection ReplaceService(
		this IServiceCollection self,
		Type serviceType,
		Type oldImplementationType,
		Type newImplementationType)
	{
		var service = self.FirstOrDefault(s => s.ServiceType == serviceType);
		if (service is null) return self;
		if (service.ImplementationType == oldImplementationType)
		{
			self.Remove(service);
			self.Add(new ServiceDescriptor(serviceType, newImplementationType, service.Lifetime));
		}

		return self;
	}

	/// <summary>
	/// Replaces a service in the service collection with a different implementation if the registered service type matches the given implementation
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TService">the type of the service</typeparam>
	/// <typeparam name="TOldImplementation">the type of the implementation to replace</typeparam>
	/// <typeparam name="TNewImplementation">the type of the new implementation</typeparam>
	public static IServiceCollection ReplaceService<
		TService,
		TOldImplementation,
		TNewImplementation>(this IServiceCollection self)
		=> ReplaceService(
			self,
			typeof(TService),
			typeof(TOldImplementation),
			typeof(TNewImplementation));

	/// <summary>
	/// Adds the necessary services to use an entity via a REST API repository
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TEntity">the type of the entity</typeparam>
	/// <typeparam name="TUrlProvider">the type of the REST URL provider</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddRestfulEntity<
		TEntity,
		TUrlProvider>(this IServiceCollection self)
		where TEntity : class
		where TUrlProvider : class, IRestfulRepositoryUrlProvider<TEntity>
		=> AddRestfulEntity<TEntity, TUrlProvider, RestfulRepository<TEntity>>(self);

	/// <summary>
	/// Adds the necessary services to use an entity via a REST API repository
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TEntity">the type of the entity</typeparam>
	/// <typeparam name="TUrlProvider">the type of the REST URL provider</typeparam>
	/// <typeparam name="TRepository">the implementation type of the repository</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddRestfulEntity<
		TEntity,
		TUrlProvider,
		TRepository>(this IServiceCollection self)
		where TUrlProvider : class, IRestfulRepositoryUrlProvider<TEntity>
		where TRepository : class, IRepository<TEntity>
		=> AddRestfulEntity<TEntity, TUrlProvider, IRepository<TEntity>, TRepository>(self);

	/// <summary>
	/// Adds the necessary services to use an entity via a REST API repository
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TEntity">the type of the entity</typeparam>
	/// <typeparam name="TUrlProvider">the type of the REST URL provider</typeparam>
	/// <typeparam name="TRepository">the service type of the repository</typeparam>
	/// <typeparam name="TRepositoryImplementation">the implementation type of the repository</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddRestfulEntity<
		TEntity,
		TUrlProvider,
		TRepository,
		TRepositoryImplementation>(this IServiceCollection self)
		where TUrlProvider : class, IRestfulRepositoryUrlProvider<TEntity>
		where TRepository : class, IRepository<TEntity>
		where TRepositoryImplementation : class, TRepository
	{
		self.TryAddScoped<IRestfulRepositoryUrlProvider<TEntity>, TUrlProvider>();
		self.TryAddScoped<TRepository, TRepositoryImplementation>();

		if (typeof(TRepository) != typeof(IRepository<TEntity>))
		{
			self.AddScoped<IRepository<TEntity>>(
				sp => sp.GetRequiredService<TRepository>());
		}

		return self;
	}

	/// <summary>
	/// Adds an access validator for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="TValidator">the validator implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddAccessValidator<TRequest, TValidator>(
		this IServiceCollection self)
		where TValidator : class, IAccessValidator<TRequest>
		=> self.AddScoped<IAccessValidator<TRequest>, TValidator>();

	/// <summary>
	/// Adds a state validator for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="TValidator">the validator implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddStateValidator<TRequest, TValidator>(
		this IServiceCollection self)
		where TValidator : class, IStateValidator<TRequest>
		=> self.AddScoped<IStateValidator<TRequest>, TValidator>();

	/// <summary>
	/// Adds a before-process hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="THook">the hook implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddBeforeHook<TRequest, THook>(
		this IServiceCollection self)
		where THook : class, IBeforeProcess<TRequest>
		=> self.AddScoped<IBeforeProcess<TRequest>, THook>();

	/// <summary>
	/// Adds an after-process hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="THook">the hook implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddAfterHook<TRequest, THook>(
		this IServiceCollection self)
		where THook : class, IAfterProcess<TRequest>
		=> self.AddScoped<IAfterProcess<TRequest>, THook>();

	/// <summary>
	/// Adds a processor
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="TResult">the data type of the result</typeparam>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddProcessor<TRequest, TResult, TProcessor>(
		this IServiceCollection self)
		where TProcessor : class, IProcessor<TRequest, TResult>
	{
		self.TryAddScoped<IProcessor<TRequest, TResult>, TProcessor>();
		return self;
	}

	/// <summary>
	/// Adds a status processor (<c>IProcessor&lt;TRequest, bool&gt;</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddStatusProcessor<TRequest, TProcessor>(
		this IServiceCollection self)
		where TProcessor : class, IProcessor<TRequest, bool>
		=> AddProcessor<TRequest, bool, TProcessor>(self);

	/// <summary>
	/// Adds a result processor (<c>IProcessor&lt;TRequest&gt;</c>)
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TResult">the data type of the result</typeparam>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddResultProcessor<TResult, TProcessor>(
        this IServiceCollection self)
		where TProcessor : class, IProcessor<TResult>
	{
		self.TryAddScoped<IProcessor<TResult>, TProcessor>();
		return self;
	}
}