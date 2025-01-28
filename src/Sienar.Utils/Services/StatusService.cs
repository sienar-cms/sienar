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
public class StatusService<TRequest> : ServiceBase, IStatusService<TRequest>
	where TRequest : IRequest
{
	private readonly ILogger<StatusService<TRequest>> _logger;
	private readonly IBotDetector _botDetector;
	private readonly IAccessValidatorService<TRequest> _accessValidator;
	private readonly IStateValidatorService<TRequest> _stateValidator;
	private readonly IBeforeActionService<TRequest> _beforeHooks;
	private readonly IAfterActionService<TRequest> _afterHooks;
	private readonly IStatusProcessor<TRequest> _processor;

	public StatusService(
		ILogger<StatusService<TRequest>> logger,
		IBotDetector botDetector,
		IAccessValidatorService<TRequest> accessValidator,
		IStateValidatorService<TRequest> stateValidator,
		IBeforeActionService<TRequest> beforeHooks,
		IAfterActionService<TRequest> afterHooks,
		IStatusProcessor<TRequest> processor,
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

	/// <inheritdoc />
	public virtual async Task<OperationResult<bool>> Execute(TRequest request)
	{
		if (request is Honeypot honeypot && _botDetector.IsSpambot(honeypot))
		{
			return NotifyOfResult(new OperationResult<bool>(result: true));
		}

		// Run access validation
		var result = await _accessValidator.Validate(request, ActionType.StatusAction);
		if (!result.Result)
		{
			return NotifyOfResult(result);
		}

		// Run state validation
		result = await _stateValidator.Validate(request, ActionType.StatusAction);
		if (!result.Result)
		{
			return NotifyOfResult(result);
		}

		// Run before hooks
		result = await _beforeHooks.Run(request, ActionType.StatusAction);
		if (!result.Result)
		{
			return NotifyOfResult(result);
		}

		try
		{
			result = await _processor.Process(request);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "{type} failed to process", typeof(IStatusProcessor<TRequest>));
			return NotifyOfResult(new OperationResult<bool>(OperationStatus.Unknown));
		}

		if (result.Status is OperationStatus.Success)
		{
			await _afterHooks.Run(request, ActionType.StatusAction);
		}

		return NotifyOfResult(result);
	}
}