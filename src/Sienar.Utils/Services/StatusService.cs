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
public class StatusService<TRequest> : ServiceBase, IStatusService<TRequest>
	where TRequest : IRequest
{
	private readonly ILogger<StatusService<TRequest>> _logger;
	private readonly IBotDetector _botDetector;
	private readonly IAccessValidationRunner<TRequest> _accessValidator;
	private readonly IStateValidationRunner<TRequest> _stateValidator;
	private readonly IBeforeActionRunner<TRequest> _beforeHooks;
	private readonly IAfterActionRunner<TRequest> _afterHooks;
	private readonly IStatusProcessor<TRequest> _processor;

	public StatusService(
		ILogger<StatusService<TRequest>> logger,
		IBotDetector botDetector,
		IAccessValidationRunner<TRequest> accessValidator,
		IStateValidationRunner<TRequest> stateValidator,
		IBeforeActionRunner<TRequest> beforeHooks,
		IAfterActionRunner<TRequest> afterHooks,
		IStatusProcessor<TRequest> processor,
		INotifier notifier)
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
		var result = await _accessValidator.Validate(request, ActionType.Status);
		if (!result.Result)
		{
			return NotifyOfResult(result);
		}

		// Run state validation
		result = await _stateValidator.Validate(request, ActionType.Status);
		if (!result.Result)
		{
			return NotifyOfResult(result);
		}

		// Run before hooks
		result = await _beforeHooks.Run(request, ActionType.Status);
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
			await _afterHooks.Run(request, ActionType.Status);
		}

		return NotifyOfResult(result);
	}
}