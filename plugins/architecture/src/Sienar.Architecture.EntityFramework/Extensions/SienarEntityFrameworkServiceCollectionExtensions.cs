using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Extensions;

public static class SienarEntityFrameworkServiceCollectionExtensions
{
	/// <summary>
	/// Adds the necessary services to use an entity via an EF repository
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TEntity">the type of the entity</typeparam>
	/// <typeparam name="TFilterProcessor">the type of the filter processor</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddEntityFrameworkEntity<
		TEntity,
		TFilterProcessor>(this IServiceCollection self)
		where TEntity : EntityBase
		where TFilterProcessor : class, IEntityFrameworkFilterProcessor<TEntity>
		=> AddEntityFrameworkEntity<TEntity, TFilterProcessor, EntityFrameworkRepository<TEntity>>(self);

	/// <summary>
	/// Adds the necessary services to use an entity via an EF repository
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TEntity">the type of the entity</typeparam>
	/// <typeparam name="TFilterProcessor">the type of the filter processor</typeparam>
	/// <typeparam name="TRepository">the implementation type of the repository</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddEntityFrameworkEntity<
		TEntity,
		TFilterProcessor,
		TRepository>(this IServiceCollection self)
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
	public static IServiceCollection AddEntityFrameworkEntity<
		TEntity,
		TFilterProcessor,
		TRepository,
		TRepositoryImplementation>(this IServiceCollection self)
		where TEntity : EntityBase
		where TFilterProcessor : class, IEntityFrameworkFilterProcessor<TEntity>
		where TRepository : class, IRepository<TEntity>
		where TRepositoryImplementation : class, TRepository
	{
		self.TryAddScoped<IBeforeProcess<TEntity>, ConcurrencyStampUpdateHook<TEntity>>();
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

}