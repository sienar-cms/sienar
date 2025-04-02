#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Sienar.Hooks;

/// <exclude />
public class AfterActionRunner<T> : IAfterActionRunner<T>
{
	private readonly IEnumerable<IAfterAction<T>> _hooks;
	private readonly ILogger<IAfterActionRunner<T>> _logger;

	public AfterActionRunner(
		IEnumerable<IAfterAction<T>> hooks,
		ILogger<IAfterActionRunner<T>> logger)
	{
		_hooks = hooks;
		_logger = logger;
	}

	public async Task Run(T input, ActionType action)
	{
		foreach (var hook in _hooks)
		{
			try
			{
				await hook.Handle(input, action);
			}
			catch (Exception e)
			{
				_logger.LogError(
					e,
					"One or more after {action} hooks failed to run",
					action);
			}
		}
	}
}
