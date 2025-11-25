using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Data;
using Sienar.Hooks;
using Sienar.Processors;

namespace Sienar.Extensions;

public static class SienarEfServiceCollectionExtensions
{
	/// <summary>
	/// Adds the necessary services to use an entity via an EF repository
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="TEntity">The type of the entity</typeparam>
	/// <typeparam name="TFilterProcessor">The type of the filter processor</typeparam>
	/// <typeparam name="TContext">The type of the filter processor</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddEfEntity<TEntity, TFilterProcessor, TContext>(
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