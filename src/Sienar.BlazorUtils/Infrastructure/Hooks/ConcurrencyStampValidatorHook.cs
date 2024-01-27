using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Services;

namespace Sienar.Infrastructure.Hooks;

public class ConcurrencyStampValidatorHook<TEntity>
	: DbService<TEntity, DbContext>, IEntityStateValidator<TEntity>
	where TEntity : EntityBase
{
	/// <inheritdoc />
	public ConcurrencyStampValidatorHook(
		IDbContextAccessor<DbContext> contextAccessor,
		ILogger<DbService<TEntity, DbContext>> logger,
		INotificationService notifier)
		: base(contextAccessor, logger, notifier) {}

	/// <inheritdoc />
	public async Task<bool> IsValid(TEntity entity, bool adding)
	{
		// Don't run on insert
		if (adding) return true;

		var concurrencyStamp = await EntitySet
			.Where(m => m.Id == entity.Id)
			.Select(m => m.ConcurrencyStamp)
			.FirstOrDefaultAsync();

		if (concurrencyStamp == Guid.Empty
			|| concurrencyStamp != entity.ConcurrencyStamp)
		{
			Notifier.Error($"Unable to update {typeof(TEntity).Name}: the entity has been updated by another user.");
			return false;
		}

		return true;
	}
}