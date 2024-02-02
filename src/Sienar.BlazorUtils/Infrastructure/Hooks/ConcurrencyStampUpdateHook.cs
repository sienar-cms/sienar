using System;
using System.Threading.Tasks;
using Sienar.Infrastructure.Entities;

namespace Sienar.Infrastructure.Hooks;

public class ConcurrencyStampUpdateHook<TEntity> : IBeforeProcess<TEntity>
	where TEntity : EntityBase
{
	/// <inheritdoc />
	public Task<HookStatus> Handle(TEntity entity, ActionType action)
	{
		if (action is ActionType.Create or ActionType.Update)
		{
			entity.ConcurrencyStamp = Guid.NewGuid();
		}

		return Task.FromResult(HookStatus.Success);
	}
}