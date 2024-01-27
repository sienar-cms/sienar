using System;
using System.Threading.Tasks;
using Sienar.Infrastructure.Entities;

namespace Sienar.Infrastructure.Hooks;

public class ConcurrencyStampUpdateHook<TEntity> : IBeforeUpsert<TEntity>
	where TEntity : EntityBase
{
	/// <inheritdoc />
	public Task<HookStatus> Handle(TEntity entity, bool isAdding)
	{
		entity.ConcurrencyStamp = Guid.NewGuid();
		return Task.FromResult(HookStatus.Success);
	}
}