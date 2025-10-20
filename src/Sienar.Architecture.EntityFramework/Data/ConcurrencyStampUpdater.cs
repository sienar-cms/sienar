#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Sienar.Hooks;

namespace Sienar.Data;

/// <exclude />
public class ConcurrencyStampUpdater<TEntity> : IBeforeAction<TEntity>
	where TEntity : EntityBase
{
	public Task Handle(TEntity entity, ActionType action)
	{
		if (action is ActionType.Create or ActionType.Update)
		{
			entity.ConcurrencyStamp = Guid.NewGuid();
		}

		return Task.CompletedTask;
	}
}