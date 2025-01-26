#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Data;
using Sienar.Hooks;

namespace Sienar.Services;

/// <exclude />
public class BeforeActionService<T> : IBeforeActionService<T>
{
	private readonly IEnumerable<IBeforeAction<T>> _hooks;
	private readonly ILogger<IBeforeActionService<T>> _logger;

	public BeforeActionService(
		IEnumerable<IBeforeAction<T>> hooks,
		ILogger<IBeforeActionService<T>> logger)
	{
		_hooks = hooks;
		_logger = logger;
	}

	public async Task<OperationResult<bool>> Run(
		T input,
		ActionType action)
	{
		try
		{
			foreach (var hook in _hooks)
			{
				await hook.Handle(input, action);
			}
		}
		catch (Exception e)
		{
			_logger.LogError(
				e,
				"One or more before {action} hooks failed to run",
				action);
			return new(
				OperationStatus.Unknown,
				false,
				StatusMessages.Processes.BeforeHookFailure);
		}

		return new(OperationStatus.Success, true);
	}
}
