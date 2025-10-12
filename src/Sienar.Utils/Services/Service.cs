#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Data;
using Sienar.Hooks;
using Sienar.Infrastructure;
using Sienar.Processors;
using Sienar.Security;

namespace Sienar.Services;

/// <exclude />
public class Service<TRequest, TResult> : ServiceBase, IService<TRequest, TResult>
	where TRequest : IRequest
	where TResult : IResult
{
	private readonly ILogger<Service<TRequest, TResult>> _logger;
	private readonly IBotDetector _botDetector;
	private readonly IAccessValidationRunner<TRequest> _accessValidator;
	private readonly IStateValidatorService<TRequest> _stateValidator;
	private readonly IBeforeActionService<TRequest> _beforeHooks;
	private readonly IAfterActionService<TRequest> _afterHooks;
	private readonly IProcessor<TRequest, TResult> _processor;

	public Service(
		ILogger<Service<TRequest, TResult>> logger,
		IBotDetector botDetector,
		IAccessValidationRunner<TRequest> accessValidator,
		IStateValidatorService<TRequest> stateValidator,
		IBeforeActionService<TRequest> beforeHooks,
		IAfterActionService<TRequest> afterHooks,
		IProcessor<TRequest, TResult> processor,
		INotificationService notifier)
		: base(notifier)
	{
		_logger = logger;
		_botDetector = botDetector;
		_accessValidator = accessValidator;
		_stateValidator = stateValidator;
		_beforeHooks = beforeHooks;
		_afterHooks = afterHooks;
		_processor = processor;
	}

	public virtual async Task<OperationResult<TResult?>> Execute(TRequest request)
	{
		if (request is Honeypot honeypot && _botDetector.IsSpambot(honeypot))
		{
			return NotifyOfResult(new OperationResult<TResult?>());
		}

		// Run access validation
		var accessValidationResult = await _accessValidator.Validate(request, ActionType.Action);
		if (!accessValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<TResult?>(
				accessValidationResult.Status,
				default,
				accessValidationResult.Message));
		}

		// Run state validation
		var stateValidationResult = await _stateValidator.Validate(request, ActionType.Action);
		if (!stateValidationResult.Result)
		{
			return NotifyOfResult(new OperationResult<TResult?>(
				stateValidationResult.Status,
				default,
				stateValidationResult.Message));
		}

		// Run before hooks
		var beforeHooksResult = await _beforeHooks.Run(request, ActionType.Action);
		if (!beforeHooksResult.Result)
		{
			return NotifyOfResult(new OperationResult<TResult?>(
				beforeHooksResult.Status,
				default,
				beforeHooksResult.Message));
		}

		OperationResult<TResult?> result;
		try
		{
			result = await _processor.Process(request);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "{type} failed to process", typeof(IProcessor<TRequest, TResult>));
			return NotifyOfResult(new OperationResult<TResult?>(OperationStatus.Unknown));
		}

		if (result.Status is OperationStatus.Success)
		{
			await _afterHooks.Run(request, ActionType.Action);
		}

		return NotifyOfResult(result);
	}
}