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
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class ResultService<TResult> : IResultService<TResult>
{
	private readonly ILogger<ResultService<TResult>> _logger;
	private readonly IEnumerable<IAccessValidator<TResult>> _accessValidators;
	private readonly IEnumerable<IAfterProcess<TResult>> _afterHooks;
	private readonly IProcessor<TResult> _processor;
	private readonly INotificationService _notifier;

	public ResultService(
		ILogger<ResultService<TResult>> logger,
		IEnumerable<IAccessValidator<TResult>> accessValidators,
		IEnumerable<IAfterProcess<TResult>> afterHooks,
		IProcessor<TResult> processor,
		INotificationService notifier)
	{
		_logger = logger;
		_accessValidators = accessValidators;
		_afterHooks = afterHooks;
		_processor = processor;
		_notifier = notifier;
	}

	public virtual async Task<TResult?> Execute()
	{
		TResult? result = default;

		if (!await _accessValidators.Validate(result, ActionType.ResultAction, _logger))
		{
			_notifier.Error(StatusMessages.Processes.NoPermission);
			return result;
		}

		try
		{
			var processResult = await _processor.Process();

			if (processResult.Status == HookStatus.Success)
			{
				_notifier.Success(processResult.Message);
			}
			else
			{
				_notifier.Error(processResult.Message);
				return processResult.Result;
			}

			result = processResult.Result;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "{type} failed to process", typeof(IProcessor<TResult>));

			_notifier.Error(StatusMessages.Processes.Unknown);
			return result;
		}

		if (result == null) return default;
		await _afterHooks.Run(result, ActionType.ResultAction, _logger);
		return result;
	}
}