#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Extensions;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Infrastructure.Services;

/// <exclude />
public class Service<TRequest, TResult> : IService<TRequest, TResult>
{
	private readonly ILogger<Service<TRequest, TResult>> _logger;
	private readonly IEnumerable<IAccessValidator<TRequest>> _accessValidators;
	private readonly IEnumerable<IStateValidator<TRequest>> _stateValidators;
	private readonly IEnumerable<IBeforeProcess<TRequest>> _beforeHooks;
	private readonly IEnumerable<IAfterProcess<TRequest>> _afterHooks;
	private readonly IProcessor<TRequest, TResult> _processor;

	public Service(
		ILogger<Service<TRequest, TResult>> logger,
		IEnumerable<IAccessValidator<TRequest>> accessValidators,
		IEnumerable<IStateValidator<TRequest>> stateValidators,
		IEnumerable<IBeforeProcess<TRequest>> beforeHooks,
		IEnumerable<IAfterProcess<TRequest>> afterHooks,
		IProcessor<TRequest, TResult> processor)
	{
		_logger = logger;
		_accessValidators = accessValidators;
		_stateValidators = stateValidators;
		_beforeHooks = beforeHooks;
		_afterHooks = afterHooks;
		_processor = processor;
	}

	public virtual async Task<TResult?> Execute(TRequest request)
	{
		if (!await _accessValidators.Validate(request, ActionType.Action, _logger))
		{
			_processor.NotifyNoPermission();
			return default;
		}

		if (!await _stateValidators.Validate(request, ActionType.Action, _logger))
		{
			_processor.NotifyFailure();
			return default;
		}

		if (!await _beforeHooks.Run(request, ActionType.Action, _logger))
		{
			_processor.NotifyFailure();
			return default;
		}

		TResult? result;
		try
		{
			var processResult = await _processor.Process(request);
			if (processResult.Status != HookStatus.Success)
			{
				// Don't notify failure because the failure was calculated
				// so the IProcessor should notify the user
				return processResult.Result;
			}

			result = processResult.Result;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "{type} failed to process", typeof(IProcessor<TRequest, TResult>));

			// Notify failure because the failure was unplanned
			_processor.NotifyFailure();
			return default;
		}

		await _afterHooks.Run(request, ActionType.Action, _logger);

		_processor.NotifySuccess();
		return result;
	}
}