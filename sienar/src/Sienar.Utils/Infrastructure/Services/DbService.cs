using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Sienar.Infrastructure.Services;

/// <summary>
/// A base type for classes that work with a specific <c>TEntity</c> in the database
/// </summary>
/// <typeparam name="TEntity">the type of the database entity</typeparam>
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

/// <summary>
/// A base type for classes that work with a specific <c>TEntity</c> in the database
/// </summary>
/// <typeparam name="TEntity">the type of the database entity</typeparam>
/// <typeparam name="TContext">the type of the database context</typeparam>
public abstract class DbService<TEntity, TContext>
	where TEntity : class
	where TContext : DbContext
{
	/// <summary>
	/// The database context
	/// </summary>
	protected readonly TContext Context;

	/// <summary>
	/// A logger for this instance
	/// </summary>
	protected readonly ILogger<DbService<TEntity, TContext>> Logger;

	/// <summary>
	/// The notification service
	/// </summary>
	protected readonly INotificationService Notifier;

	/// <summary>
	/// The <see cref="DbSet{TEntity}"/> for this <c>TEntity</c>
	/// </summary>
	protected DbSet<TEntity> EntitySet => Context.Set<TEntity>();

	/// <summary>
	/// Creates a new <c>DbService</c>
	/// </summary>
	/// <param name="context">the database context</param>
	/// <param name="logger">the logger for this instance</param>
	/// <param name="notifier">the notification service</param>
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