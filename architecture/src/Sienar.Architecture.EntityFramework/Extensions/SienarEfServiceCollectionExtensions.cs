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
		self.TryAddScoped<IEntityWriter<TEntity>, EfEntityWriter<TEntity, TContext>>();
		self.TryAddScoped<IEntityDeleter<TEntity>, EfEntityDeleter<TEntity, TContext>>();

		return self;
	}

	/// <summary>
	/// Adds the necessary services to use an entity via Entity Framework
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="TDto">The type of the DTO</typeparam>
	/// <typeparam name="TMapper">The type of the DTO mapper</typeparam>
	/// <typeparam name="TEntity">The type of the entity</typeparam>
	/// <typeparam name="TFilterProcessor">The type of the filter processor</typeparam>
	/// <typeparam name="TContext">The type of the <see cref="DbContext"/></typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddEfEntity<
		TDto,
		TMapper,
		TEntity,
		TFilterProcessor,
		TContext>(this IServiceCollection self)
		where TDto : class, new()
		where TMapper : class, IMapper<TDto, TEntity>
		where TEntity : EntityBase, new()
		where TFilterProcessor : class, IEntityFrameworkFilterProcessor<TEntity>
		where TContext : DbContext
		=> AddEfEntity<TDto, TMapper, TDto, TMapper, TDto, TMapper, TEntity, TFilterProcessor, TContext>(self);

	/// <summary>
	/// Adds the necessary services to use an entity via Entity Framework
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="TViewDto">The type of the view DTO</typeparam>
	/// <typeparam name="TViewDtoMapper">The type of the view DTO mapper</typeparam>
	/// <typeparam name="TAddDto">The type of the add DTO</typeparam>
	/// <typeparam name="TAddDtoMapper">The type of the add DTO mapper</typeparam>
	/// <typeparam name="TEditDto">The type of the edit DTO</typeparam>
	/// <typeparam name="TEditDtoMapper">The type of the edit DTO mapper</typeparam>
	/// <typeparam name="TEntity">The type of the entity</typeparam>
	/// <typeparam name="TFilterProcessor">The type of the filter processor</typeparam>
	/// <typeparam name="TContext">The type of the <see cref="DbContext"/></typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddEfEntity<
		TViewDto,
		TViewDtoMapper,
		TAddDto,
		TAddDtoMapper,
		TEditDto,
		TEditDtoMapper,
		TEntity,
		TFilterProcessor,
		TContext>(this IServiceCollection self)
		where TViewDto : class, new()
		where TViewDtoMapper : class, IMapper<TViewDto, TEntity>
		where TAddDto : class, new()
		where TAddDtoMapper : class, IMapper<TAddDto, TEntity>
		where TEditDto : class, new()
		where TEditDtoMapper : class, IMapper<TEditDto, TEntity>
		where TEntity : EntityBase, new()
		where TFilterProcessor : class, IEntityFrameworkFilterProcessor<TEntity>
		where TContext : DbContext
	{
		self.TryAddScoped<IMapper<TViewDto, TEntity>, TViewDtoMapper>();

		if (typeof(TAddDtoMapper) != typeof(TViewDtoMapper))
		{
			self.TryAddScoped<IMapper<TAddDto, TEntity>, TAddDtoMapper>();
		}

		if (typeof(TEditDtoMapper) != typeof(TViewDtoMapper) && typeof(TEditDtoMapper) != typeof(TAddDtoMapper))
		{
			self.TryAddScoped<IMapper<TEditDto, TEntity>, TEditDtoMapper>();
		}

		self.TryAddScoped<IBeforeAction<TEntity>, ConcurrencyStampUpdater<TEntity>>();
		self.TryAddScoped<IStateValidator<TEntity>, ConcurrencyStampValidator<TEntity, TContext>>();
		self.TryAddScoped<IEntityFrameworkFilterProcessor<TEntity>, TFilterProcessor>();
		self.TryAddScoped<IEntityReader<TEntity>, EfEntityReader<TEntity, TContext>>();
		self.TryAddScoped<IEntityWriter<TEntity>, EfEntityWriter<TEntity, TContext>>();
		self.TryAddScoped<IEntityDeleter<TEntity>, EfEntityDeleter<TEntity, TContext>>();

		return self;
	}
}