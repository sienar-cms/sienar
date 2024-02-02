using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Extensions;

public static class HooksExtensions
{
	public static async Task<bool> Run<TEntity>(
		this IEnumerable<IBeforeProcess<TEntity>> beforeHooks,
		TEntity entity,
		ActionType action,
		ILogger logger)
	{
		var wasSuccessful = true;

		try
		{
			foreach (var beforeHook in beforeHooks)
			{
				if (await beforeHook.Handle(entity, action) != HookStatus.Success) wasSuccessful = false;
			}
		}
		catch (Exception e)
		{
			logger.LogError(
				e,
				"One or more before {action} hooks failed to run",
				action);
			return false;
		}

		return wasSuccessful;
	}

	public static async Task Run<TEntity>(
		this IEnumerable<IAfterProcess<TEntity>> afterHooks,
		TEntity entity,
		ActionType action,
		ILogger logger)
	{
		foreach (var afterHook in afterHooks)
		{
			try
			{
				await afterHook.Handle(entity, action);
			}
			catch (Exception e)
			{
				logger.LogError(
					e,
					"One or more after {action} hooks failed to run",
					action);
			}
		}
	}

	public static async Task<bool> Run<TEntity>(
		this IEnumerable<IEntityStateValidator<TEntity>> stateValidators,
		TEntity entity,
		bool isAdding,
		ILogger logger)
	{
		try
		{
			var wasSuccessful = true;
			foreach (var validator in stateValidators)
			{
				if (!await validator.IsValid(entity, isAdding)) wasSuccessful = false;
			}

			return wasSuccessful;
		}
		catch (Exception e)
		{
			logger.LogError(e, "One or more state validators failed to run");
			return false;
		}
	}

	public static async Task<bool> Validate<TEntity>(
		this IEnumerable<IAccessValidator<TEntity>> accessValidators,
		TEntity entity,
		ActionType action,
		ILogger logger)
	{
		var context = new AccessValidationContext();
		var anyValidators = false;

		try
		{
			foreach (var validator in accessValidators)
			{
				anyValidators = true;
				await validator.Validate(context, action, entity);
			}
		}
		catch (Exception e)
		{
			logger.LogError(e, "One or more access validators failed to run");
			return false;
		}

		return !anyValidators || context.CanAccess;
	}
}