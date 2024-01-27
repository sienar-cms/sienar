using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Infrastructure.Services;

public class HookableService<TRequest> : IHookableService<TRequest>
{
	private readonly ILogger<HookableService<TRequest>> _logger;
	private readonly IEnumerable<IBeforeProcess<TRequest>> _beforeHooks;
	private readonly IEnumerable<IAfterProcess<TRequest>> _afterHooks;
	private readonly IProcessor<TRequest> _processor;

	public HookableService(
		ILogger<HookableService<TRequest>> logger,
		IEnumerable<IBeforeProcess<TRequest>> beforeHooks,
		IEnumerable<IAfterProcess<TRequest>> afterHooks,
		IProcessor<TRequest> processor)
	{
		_logger = logger;
		_beforeHooks = beforeHooks;
		_afterHooks = afterHooks;
		_processor = processor;
	}

	/// <inheritdoc />
	public virtual async Task<bool> Execute(TRequest request)
	{
		if (!await RunBeforeHooks(request))
		{
			_processor.NotifyBeforeHookFailure();
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
			_processor.NotifyProcessFailure();
			return false;
		}

		if (!await RunAfterHooks(request))
		{
			_processor.NotifyAfterHookFailure();
		}

		_processor.NotifySuccess();
		return true;
	}

	private async Task<bool> RunBeforeHooks(TRequest model)
	{
		var successful = true;

		try
		{
			foreach (var hook in _beforeHooks)
			{
				if (await hook.Handle(model) != HookStatus.Success) successful = false;
			}
		}
		catch (Exception e)
		{
			_logger.LogError(e, "One or more before hooks failed to run");
			successful = false;
		}

		return successful;
	}

	private async Task<bool> RunAfterHooks(TRequest model)
	{
		try
		{
			foreach (var hook in _afterHooks)
			{
				await hook.Handle(model);
			}
		}
		catch (Exception e)
		{
			_logger.LogError(e, "One or more after hooks failed to run");
			return false;
		}

		return true;
	}
}