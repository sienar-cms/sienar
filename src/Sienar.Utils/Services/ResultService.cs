#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Data;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Processors;

namespace Sienar.Services;

/// <exclude />
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class ResultService<TResult> : ServiceBase, IResultService<TResult>
{
	private readonly ILogger<ResultService<TResult>> _logger;
	private readonly IAccessValidatorService<TResult> _accessValidator;
	private readonly IAfterActionService<TResult> _afterHooks;
	private readonly IProcessor<TResult> _processor;

	public ResultService(
		ILogger<ResultService<TResult>> logger,
		IAccessValidatorService<TResult> accessValidator,
		IAfterActionService<TResult> afterHooks,
		IProcessor<TResult> processor,
		INotificationService notifier)
		: base(notifier)
	{
		_logger = logger;
		_accessValidator = accessValidator;
		_afterHooks = afterHooks;
		_processor = processor;
	}

	public virtual async Task<OperationResult<TResult?>> Execute()
	{
		// Run access validation
		var accessValidationResult = await _accessValidator.Validate(default, ActionType.ResultAction);
		if (!accessValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<TResult?>(
				accessValidationResult.Status,
				default,
				accessValidationResult.Message));
		}

		OperationResult<TResult?> result;
		try
		{
			result = await _processor.Process();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "{type} failed to process", typeof(IProcessor<TResult>));
			return NotifyOfResult(new OperationResult<TResult?>(OperationStatus.Unknown));
		}

		if (result.Status is OperationStatus.Success && result.Result is not null)
		{
			await _afterHooks.Run(result.Result, ActionType.ResultAction);
		}

		return NotifyOfResult(result);
	}
}