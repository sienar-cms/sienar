using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Extensions;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Infrastructure.Services;

public class Service<TRequest> : IService<TRequest>
{
	private readonly ILogger<Service<TRequest>> _logger;
	private readonly IEnumerable<IAccessValidator<TRequest>> _accessValidators;
	private readonly IEnumerable<IStateValidator<TRequest>> _stateValidators;
	private readonly IEnumerable<IBeforeProcess<TRequest>> _beforeHooks;
	private readonly IEnumerable<IAfterProcess<TRequest>> _afterHooks;
	private readonly IProcessor<TRequest> _processor;

	public Service(
		ILogger<Service<TRequest>> logger,
		IEnumerable<IAccessValidator<TRequest>> accessValidators,
		IEnumerable<IStateValidator<TRequest>> stateValidators,
		IEnumerable<IBeforeProcess<TRequest>> beforeHooks,
		IEnumerable<IAfterProcess<TRequest>> afterHooks,
		IProcessor<TRequest> processor)
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
		if (!await _accessValidators.Validate(request, ActionType.Action, _logger))
		{
			_processor.NotifyNoPermission();
			return false;
		}

		if (!await _stateValidators.Run(request, ActionType.Action, _logger))
		{
			_processor.NotifyFailure();
			return false;
		}

		if (!await _beforeHooks.Run(request, ActionType.Action, _logger))
		{
			_processor.NotifyFailure();
			return false;
		}

		try
		{
			if (await _processor.Process(request) != HookStatus.Success)
			{
				// Don't notify failure because the failure was calculated
				// so the IProcessor should notify the user
				return false;
			}
		}
		catch (Exception e)
		{
			_logger.LogError(e, "{type} failed to process", typeof(IProcessor<TRequest>));

			// Notify failure because the failure was unplanned
			_processor.NotifyFailure();
			return false;
		}

		await _afterHooks.Run(request, ActionType.Action, _logger);

		_processor.NotifySuccess();
		return true;
	}
}