using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sienar.Data;
using Sienar.Email;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Processors;
using Sienar.Services;
using Sienar.Ui;

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
	/// <returns>the service collection</returns>
	[ExcludeFromCodeCoverage]
	public static IServiceCollection AddSienarCoreUtilities(this IServiceCollection self)
	{
		self.TryAddScoped(typeof(IEntityReader<>), typeof(EntityReader<>));
		self.TryAddScoped(typeof(IEntityWriter<>), typeof(EntityWriter<>));
		self.TryAddScoped(typeof(IEntityDeleter<>), typeof(EntityDeleter<>));
		self.TryAddScoped(typeof(IStatusService<>), typeof(StatusService<>));
		self.TryAddScoped(typeof(IService<,>), typeof(Service<,>));
		self.TryAddScoped(typeof(IResultService<>), typeof(ResultService<>));
		self.TryAddScoped(typeof(IAccessValidatorService<>), typeof(AccessValidatorService<>));
		self.TryAddScoped(typeof(IStateValidatorService<>), typeof(StateValidatorService<>));
		self.TryAddScoped(typeof(IBeforeActionService<>), typeof(BeforeActionService<>));
		self.TryAddScoped(typeof(IAfterActionService<>), typeof(AfterActionService<>));
		self.TryAddScoped<IBotDetector, BotDetector>();
		self.TryAddScoped<IMenuGenerator, MenuGenerator>();
		self.TryAddScoped<IEmailSender, DefaultEmailSender>();
		self.TryAddScoped<AuthStateProvider>();
		self.TryAddScoped<AuthenticationStateProvider>(
			sp => sp.GetRequiredService<AuthStateProvider>());

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
	/// Adds a configurer of type <c>IConfigurer&lt;TOptions&gt;</c> for the given <c>TOptions</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TConfigurer">the type of the configurer</typeparam>
	/// <typeparam name="TOptions">the type of the options class to configure</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddConfigurer<TConfigurer, TOptions>(this IServiceCollection self)
		where TConfigurer : class, IConfigurer<TOptions>
		where TOptions : class
		=> self.AddScoped<IConfigurer<TOptions>, TConfigurer>();

	/// <summary>
	/// Adds a configurer of type <c>IConfigurer&lt;TOptions&gt;</c> for the given <c>TOptions</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TConfigurer">the type of the configurer</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddConfigurer<TConfigurer>(this IServiceCollection self)
		=> self.AddImplementationAsInterface(
			typeof(TConfigurer),
			typeof(IConfigurer<>),
			ServiceLifetime.Scoped,
			false);

	/// <summary>
	/// Adds a configurer of type <c>IConfigurer&lt;TOptions&gt;</c> for the given <c>TOptions</c> if one hasn't already been registered
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TConfigurer">the type of the configurer</typeparam>
	/// <typeparam name="TOptions">the type of the options class to configure</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddConfigurer<TConfigurer, TOptions>(this IServiceCollection self)
		where TConfigurer : class, IConfigurer<TOptions>
		where TOptions : class
	{
		self.TryAddScoped<IConfigurer<TOptions>, TConfigurer>();
		return self;
	}

	/// <summary>
	/// Adds a configurer of type <c>IConfigurer&lt;TOptions&gt;</c> for the given <c>TOptions</c> if one hasn't already been registered
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TConfigurer">the type of the configurer</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddConfigurer<TConfigurer>(this IServiceCollection self)
		=> self.AddImplementationAsInterface(
			typeof(TConfigurer),
			typeof(IConfigurer<>),
			ServiceLifetime.Scoped,
			true);

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
	/// Adds an access validator for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TValidator">the validator implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddAccessValidator<TValidator>(
		this IServiceCollection self)
		=> self.AddImplementationAsInterface(
			typeof(TValidator),
			typeof(IAccessValidator<>),
			ServiceLifetime.Scoped,
			false);

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
	/// Adds a state validator for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TValidator">the validator implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddStateValidator<TValidator>(
		this IServiceCollection self)
		=> self.AddImplementationAsInterface(
			typeof(TValidator),
			typeof(IStateValidator<>),
			ServiceLifetime.Scoped,
			false);

	/// <summary>
	/// Adds a before-action hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="THook">the hook implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddBeforeActionHook<TRequest, THook>(
		this IServiceCollection self)
		where THook : class, IBeforeAction<TRequest>
		=> self.AddScoped<IBeforeAction<TRequest>, THook>();

	/// <summary>
	/// Adds a before-action hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="THook">the hook implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddBeforeActionHook<THook>(
		this IServiceCollection self)
		=> self.AddImplementationAsInterface(
			typeof(THook),
			typeof(IBeforeAction<>),
			ServiceLifetime.Scoped,
			false);

	/// <summary>
	/// Adds a before-task hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="THook">the hook implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddBeforeTaskHook<TRequest, THook>(
		this IServiceCollection self)
		where TRequest : class
		where THook : class, IBeforeTask<TRequest>
		=> self.AddScoped<IBeforeTask<TRequest>, THook>();

	/// <summary>
	/// Adds a before-task hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="THook">the hook implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddBeforeTaskHook<THook>(this IServiceCollection self)
		=> self.AddImplementationAsInterface(
			typeof(THook),
			typeof(IBeforeTask<>),
			ServiceLifetime.Scoped,
			false);

	/// <summary>
	/// Adds an after-action hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="THook">the hook implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddAfterActionHook<TRequest, THook>(
		this IServiceCollection self)
		where THook : class, IAfterAction<TRequest>
		=> self.AddScoped<IAfterAction<TRequest>, THook>();

	/// <summary>
	/// Adds an after-action hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="THook">the hook implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddAfterActionHook<THook>(
		this IServiceCollection self)
		=> self.AddImplementationAsInterface(
			typeof(THook),
			typeof(IAfterAction<>),
			ServiceLifetime.Scoped,
			false);

	/// <summary>
	/// Adds an after-task hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="THook">the hook implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddAfterTaskHook<TRequest, THook>(
		this IServiceCollection self)
		where TRequest : class
		where THook : class, IAfterTask<TRequest>
		=> self.AddScoped<IAfterTask<TRequest>, THook>();

	/// <summary>
	/// Adds an after-action hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="THook">the hook implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddAfterTaskHook<THook>(
		this IServiceCollection self)
		=> self.AddImplementationAsInterface(
			typeof(THook),
			typeof(IAfterTask<>),
			ServiceLifetime.Scoped,
			false);

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
		where TRequest : IRequest
		where TResult : IResult
		=> self.AddScoped<IProcessor<TRequest, TResult>, TProcessor>();

	/// <summary>
	/// Adds a processor
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddProcessor<TProcessor>(
		this IServiceCollection self)
		=> self.AddImplementationAsInterface(
			typeof(TProcessor),
			typeof(IProcessor<,>),
			ServiceLifetime.Scoped,
			false);

	/// <summary>
	/// Adds a processor
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="TResult">the data type of the result</typeparam>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddProcessor<TRequest, TResult, TProcessor>(this IServiceCollection self)
		where TProcessor : class, IProcessor<TRequest, TResult>
		where TRequest : IRequest
		where TResult : IResult
	{
		self.TryAddScoped<IProcessor<TRequest, TResult>, TProcessor>();
		return self;
	}

	/// <summary>
	/// Adds a processor
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddProcessor<TProcessor>(
		this IServiceCollection self)
		=> self.AddImplementationAsInterface(
			typeof(TProcessor),
			typeof(IProcessor<,>),
			ServiceLifetime.Scoped,
			true);

	/// <summary>
	/// Adds a task to run once the Blazor UI has rendered and is ready to execute JavaScript
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="T">The type of the startup task</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddStartupTask<T>(this IServiceCollection self)
		where T : class, IBeforeTask<SienarStartupActor>
		=> self.AddScoped<IBeforeTask<SienarStartupActor>, T>();

	/// <summary>
	/// Adds a status processor (<c>IProcessor&lt;TRequest, bool&gt;</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddStatusProcessor<TRequest, TProcessor>(
		this IServiceCollection self)
		where TProcessor : class, IStatusProcessor<TRequest>
		where TRequest : IRequest
		=> self.AddScoped<IStatusProcessor<TRequest>, TProcessor>();

	/// <summary>
	/// Adds a status processor (<c>IProcessor&lt;TRequest, bool&gt;</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddStatusProcessor<TProcessor>(
		this IServiceCollection self)
		=> self.AddImplementationAsInterface(
			typeof(TProcessor),
			typeof(IStatusProcessor<>),
			ServiceLifetime.Scoped,
			false);

	/// <summary>
	/// Adds a status processor (<c>IProcessor&lt;TRequest, bool&gt;</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddStatusProcessor<TRequest, TProcessor>(
		this IServiceCollection self)
		where TProcessor : class, IStatusProcessor<TRequest>
		where TRequest : IRequest
	{
		self.TryAddScoped<IStatusProcessor<TRequest>>();
		return self;
	}

	/// <summary>
	/// Adds a status processor (<c>IProcessor&lt;TRequest, bool&gt;</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddStatusProcessor<TProcessor>(
		this IServiceCollection self)
		=> self.AddImplementationAsInterface(
			typeof(TProcessor),
			typeof(IStatusProcessor<>),
			ServiceLifetime.Scoped,
			true);

	/// <summary>
	/// Adds a result processor (<c>IProcessor&lt;TRequest&gt;</c>)
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TResult">the data type of the result</typeparam>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddResultProcessor<TResult, TProcessor>(
		this IServiceCollection self)
		where TProcessor : class, IResultProcessor<TResult>
		where TResult : IResult
		=> self.AddScoped<IResultProcessor<TResult>, TProcessor>();

	/// <summary>
	/// Adds a result processor (<c>IProcessor&lt;TRequest&gt;</c>)
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddResultProcessor<TProcessor>(
		this IServiceCollection self)
		=> self.AddImplementationAsInterface(
			typeof(TProcessor),
			typeof(IResultProcessor<>),
			ServiceLifetime.Scoped,
			false);

	/// <summary>
	/// Adds a result processor (<c>IProcessor&lt;TRequest&gt;</c>)
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TResult">the data type of the result</typeparam>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddResultProcessor<TResult, TProcessor>(
		this IServiceCollection self)
		where TProcessor : class, IResultProcessor<TResult>
		where TResult : IResult
	{
		self.TryAddScoped<IResultProcessor<TResult>, TProcessor>();
		return self;
	}

	/// <summary>
	/// Adds a result processor (<c>IProcessor&lt;TRequest&gt;</c>)
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddResultProcessor<TProcessor>(
		this IServiceCollection self)
		=> self.AddImplementationAsInterface(
			typeof(TProcessor),
			typeof(IResultProcessor<>),
			ServiceLifetime.Scoped,
			true);

	/// <summary>
	/// Adds the necessary services to use an entity via an EF repository
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TEntity">the type of the entity</typeparam>
	/// <typeparam name="TFilterProcessor">the type of the filter processor</typeparam>
	/// <typeparam name="TContext">the type of the database context</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddEntityFrameworkEntityWithDefaultRepository<TEntity, TFilterProcessor, TContext>(
		this IServiceCollection self)
		where TEntity : EntityBase
		where TFilterProcessor : class, IEntityFrameworkFilterProcessor<TEntity>
		where TContext : DbContext
		=> AddEntityFrameworkEntity<TEntity, TFilterProcessor, EntityFrameworkRepository<TEntity, TContext>>(self);

	/// <summary>
	/// Adds the necessary services to use an entity via an EF repository
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TEntity">the type of the entity</typeparam>
	/// <typeparam name="TFilterProcessor">the type of the filter processor</typeparam>
	/// <typeparam name="TRepository">the implementation type of the repository</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddEntityFrameworkEntity<TEntity, TFilterProcessor, TRepository>(
		this IServiceCollection self)
		where TEntity : EntityBase
		where TFilterProcessor : class, IEntityFrameworkFilterProcessor<TEntity>
		where TRepository : class, IRepository<TEntity>
		=> AddEntityFrameworkEntity<TEntity, TFilterProcessor, IRepository<TEntity>, TRepository>(self);

	/// <summary>
	/// Adds the necessary services to use an entity via an EF repository
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TEntity">the type of the entity</typeparam>
	/// <typeparam name="TFilterProcessor">the type of the filter processor</typeparam>
	/// <typeparam name="TRepository">the service type of the repository</typeparam>
	/// <typeparam name="TRepositoryImplementation">the implementation type of the repository</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddEntityFrameworkEntity<TEntity, TFilterProcessor, TRepository, TRepositoryImplementation>(
		this IServiceCollection self)
		where TEntity : EntityBase
		where TFilterProcessor : class, IEntityFrameworkFilterProcessor<TEntity>
		where TRepository : class, IRepository<TEntity>
		where TRepositoryImplementation : class, TRepository
	{
		self.TryAddScoped<IBeforeAction<TEntity>, ConcurrencyStampUpdateHook<TEntity>>();
		self.TryAddScoped<IStateValidator<TEntity>, ConcurrencyStampValidator<TEntity>>();
		self.TryAddScoped<IEntityFrameworkFilterProcessor<TEntity>, TFilterProcessor>();
		self.TryAddScoped<TRepository, TRepositoryImplementation>();

		if (typeof(TRepository) != typeof(IRepository<TEntity>))
		{
			self.AddScoped<IRepository<TEntity>>(
				sp => sp.GetRequiredService<TRepository>());
		}

		return self;
	}

	private static IServiceCollection AddImplementationAsInterface(
		this IServiceCollection self,
		Type implementationType,
		Type interfaceType,
		ServiceLifetime lifetime,
		bool tryAdd)
	{
		var concreteInterfaceTypes = implementationType
			.GetInterfaces()
			.Where(
				i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
		var implementsSpecifiedInterface = false;

		foreach (var t in concreteInterfaceTypes)
		{
			implementsSpecifiedInterface = true;
			var descriptor = new ServiceDescriptor(t, implementationType, lifetime);
			if (tryAdd)
			{
				self.TryAdd(descriptor);
			}
			else
			{
				self.Add(descriptor);
			}
		}

		if (!implementsSpecifiedInterface)
		{
			throw new InvalidOperationException($"Type {implementationType} does not inherit from {interfaceType}.");
		}

		return self;
	}
}