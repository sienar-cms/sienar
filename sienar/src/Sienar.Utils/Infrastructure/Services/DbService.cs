using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Sienar.Infrastructure.Services;

public abstract class DbService<TEntity> : DbService<TEntity, DbContext>
	where TEntity : class
{
	/// <inheritdoc />
	protected DbService(
		DbContext context,
		ILogger<DbService<TEntity, DbContext>> logger,
		INotificationService notifier)
		: base(context, logger, notifier) {}
}

public abstract class DbService<TEntity, TContext>
	where TEntity : class
	where TContext : DbContext
{
	protected readonly TContext Context;
	protected readonly ILogger<DbService<TEntity, TContext>> Logger;
	protected readonly INotificationService Notifier;
	protected DbSet<TEntity> EntitySet => Context.Set<TEntity>();

	protected DbService(
		TContext context,
		ILogger<DbService<TEntity, TContext>> logger,
		INotificationService notifier)
	{
		Context = context;
		Logger = logger;
		Notifier = notifier;
	}
}