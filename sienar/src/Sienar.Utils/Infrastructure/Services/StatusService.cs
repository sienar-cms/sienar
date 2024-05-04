#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Extensions;
using Sienar.Infrastructure.Data;
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
	private readonly INotificationService _notifier;

	public StatusService(
		ILogger<StatusService<TRequest>> logger,
		IEnumerable<IAccessValidator<TRequest>> accessValidators,
		IEnumerable<IStateValidator<TRequest>> stateValidators,
		IEnumerable<IBeforeProcess<TRequest>> beforeHooks,
		IEnumerable<IAfterProcess<TRequest>> afterHooks,
		IProcessor<TRequest, bool> processor,
		INotificationService notifier)
	{
		_logger = logger;
		_accessValidators = accessValidators;
		_stateValidators = stateValidators;
		_beforeHooks = beforeHooks;
		_afterHooks = afterHooks;
		_processor = processor;
		_notifier = notifier;
	}

	/// <inheritdoc />
	public virtual async Task<bool> Execute(TRequest request)
	{
		if (!await _accessValidators.Validate(request, ActionType.StatusAction, _logger))
		{
			_notifier.Error(StatusMessages.General.Unauthorized);
			return false;
		}

		if (!await _stateValidators.Validate(request, ActionType.StatusAction, _logger))
		{
			_notifier.Error(StatusMessages.Processes.InvalidState);
			return false;
		}

		if (!await _beforeHooks.Run(request, ActionType.StatusAction, _logger))
		{
			_notifier.Error(StatusMessages.Processes.BeforeHookFailure);
			return false;
		}

		bool result;
		try
		{
			var processResult = await _processor.Process(request);
			if (processResult.Status == OperationStatus.Success)
			{
				_notifier.Success(processResult.Message);
			}
			else
			{
				_notifier.Error(processResult.Message);
				return false;
			}

			result = processResult.Result;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "{type} failed to process", typeof(IProcessor<TRequest>));

			// Notify failure because the failure was unplanned
			_notifier.Error(StatusMessages.General.Unknown);
			return false;
		}

		await _afterHooks.Run(request, ActionType.StatusAction, _logger);
		return result;
	}
}