using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Data;
using Sienar.Hooks;
using Sienar.Processors;

namespace Sienar.Extensions;

public static class SienarEntityFrameworkServiceCollectionExtensions
{
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
	/// <param name="self">The service collection</param>
	/// <typeparam name="TEntity">The type of the entity</typeparam>
	/// <typeparam name="TFilterProcessor">The type of the filter processor</typeparam>
	/// <typeparam name="TContext">The type of the filter processor</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddEntityFrameworkEntity<TEntity, TFilterProcessor, TContext>(
		this IServiceCollection self)
		where TEntity : EntityBase
		where TFilterProcessor : class, IEntityFrameworkFilterProcessor<TEntity>
		where TContext : DbContext
	{
		self.TryAddScoped<IBeforeAction<TEntity>, ConcurrencyStampUpdater<TEntity>>();
		self.TryAddScoped<IStateValidator<TEntity>, ConcurrencyStampValidator<TEntity, TContext>>();
		self.TryAddScoped<IEntityFrameworkFilterProcessor<TEntity>, TFilterProcessor>();
		self.TryAddScoped<IEntityReader<TEntity>, EfEntityReader<TEntity, TContext>>();

		return self;
	}

}