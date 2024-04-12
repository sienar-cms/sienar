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
public class StatusService<TRequest> : IStatusService<TRequest>
{
	private readonly ILogger<StatusService<TRequest>> _logger;
	private readonly IEnumerable<IAccessValidator<TRequest>> _accessValidators;
	private readonly IEnumerable<IStateValidator<TRequest>> _stateValidators;
	private readonly IEnumerable<IBeforeProcess<TRequest>> _beforeHooks;
	private readonly IEnumerable<IAfterProcess<TRequest>> _afterHooks;
	private readonly IProcessor<TRequest, bool> _processor;

	public StatusService(
		ILogger<StatusService<TRequest>> logger,
		IEnumerable<IAccessValidator<TRequest>> accessValidators,
		IEnumerable<IStateValidator<TRequest>> stateValidators,
		IEnumerable<IBeforeProcess<TRequest>> beforeHooks,
		IEnumerable<IAfterProcess<TRequest>> afterHooks,
		IProcessor<TRequest, bool> processor)
	{
		_logger = logger;
		_accessValidators = accessValidators;
		_stateValidators = stateValidators;
		_beforeHooks = beforeHooks;
		_afterHooks = afterHooks;
		_processor = processor;
	}

	/// <inheritdoc />
	public virtual async Task<bool> Execute(TRequest request)
	{
		if (!await _accessValidators.Validate(request, ActionType.StatusAction, _logger))
		{
			_processor.NotifyNoPermission();
			return false;
		}

		if (!await _stateValidators.Validate(request, ActionType.StatusAction, _logger))
		{
			_processor.NotifyFailure();
			return false;
		}

		if (!await _beforeHooks.Run(request, ActionType.StatusAction, _logger))
		{
			_processor.NotifyFailure();
			return false;
		}

		bool result;
		try
		{
			var processResult = await _processor.Process(request);
			if (processResult.Status != HookStatus.Success)
			{
				// Don't notify failure because the failure was calculated
				// so the IProcessor should notify the user
				return false;
			}

			result = processResult.Result;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "{type} failed to process", typeof(IProcessor<TRequest>));

			// Notify failure because the failure was unplanned
			_processor.NotifyFailure();
			return false;
		}

		await _afterHooks.Run(request, ActionType.StatusAction, _logger);

		_processor.NotifySuccess();
		return result;
	}
}