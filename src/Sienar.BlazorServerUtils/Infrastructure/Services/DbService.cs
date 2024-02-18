using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Sienar.Infrastructure.Services;

public abstract class DbService<TEntity> : DbService<TEntity, DbContext>
	where TEntity : class
{
	/// <inheritdoc />
	protected DbService(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<TEntity, DbContext>> logger,
		INotificationService notifier)
		: base(contextAccessor, logger, notifier) {}
}

public abstract class DbService<TEntity, TContext>
	where TEntity : class
	where TContext : DbContext
{
	private readonly IDbContextAccessor<TContext> _contextAccessor;
	protected readonly ILogger<DbService<TEntity, TContext>> Logger;
	protected readonly INotificationService Notifier;

	protected TContext Context => _contextAccessor.Context;
	protected DbSet<TEntity> EntitySet => Context.Set<TEntity>();

	protected DbService(
		IDbContextAccessor<TContext> contextAccessor,
		ILogger<DbService<TEntity, TContext>> logger,
		INotificationService notifier)
	{
		_contextAccessor = contextAccessor;
		Logger = logger;
		Notifier = notifier;
	}
}