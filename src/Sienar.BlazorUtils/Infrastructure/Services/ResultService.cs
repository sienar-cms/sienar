using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Infrastructure.Services;

public class ResultService<TResult> : IResultService<TResult>
{
	private readonly ILogger<ResultService<TResult>> _logger;
	private readonly IEnumerable<IAfterProcess<TResult>> _afterHooks;
	private readonly IResultProcessor<TResult> _processor;

	public ResultService(
		ILogger<ResultService<TResult>> logger,
		IEnumerable<IAfterProcess<TResult>> afterHooks,
		IResultProcessor<TResult> processor)
	{
		_logger = logger;
		_afterHooks = afterHooks;
		_processor = processor;
	}

	public virtual async Task<TResult?> Execute()
	{
		TResult? result = default;
		try
		{
			(var status, result) = await _processor.Process();
			if (status != HookStatus.Success) return default;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "{type} failed to process", typeof(IResultProcessor<TResult>));

			_processor.NotifyProcessFailure();
			return result;
		}

		if (!await RunAfterHooks(result!))
		{
			return default;
		}

		_processor.NotifySuccess();
		return result;
	}

	private async Task<bool> RunAfterHooks(TResult result)
	{
		try
		{
			foreach (var hook in _afterHooks)
			{
				await hook.Handle(result);
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