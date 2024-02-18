using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Extensions;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Infrastructure.Services;

public class ResultService<TResult> : IResultService<TResult>
{
	private readonly ILogger<ResultService<TResult>> _logger;
	private readonly IEnumerable<IAccessValidator<TResult>> _accessValidators;
	private readonly IEnumerable<IAfterProcess<TResult>> _afterHooks;
	private readonly IResultProcessor<TResult> _processor;

	public ResultService(
		ILogger<ResultService<TResult>> logger,
		IEnumerable<IAccessValidator<TResult>> accessValidators,
		IEnumerable<IAfterProcess<TResult>> afterHooks,
		IResultProcessor<TResult> processor)
	{
		_logger = logger;
		_accessValidators = accessValidators;
		_afterHooks = afterHooks;
		_processor = processor;
	}

	public virtual async Task<TResult?> Execute()
	{
		TResult? result = default;

		if (!await _accessValidators.Validate(result, ActionType.ResultAction, _logger))
		{
			_processor.NotifyNoPermission();
			return result;
		}

		try
		{
			(var status, result) = await _processor.Process();
			if (status != HookStatus.Success) return default;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "{type} failed to process", typeof(IResultProcessor<TResult>));

			_processor.NotifyFailure();
			return result;
		}

		if (result == null) return default;
		await _afterHooks.Run(result, ActionType.ResultAction, _logger);

		_processor.NotifySuccess();
		return result;
	}
}