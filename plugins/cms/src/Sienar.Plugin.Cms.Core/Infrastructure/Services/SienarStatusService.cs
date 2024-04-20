using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Infrastructure.Services;

public class SienarStatusService<TRequest> : StatusService<TRequest>
{
	private readonly IBotDetector _botDetector;

	/// <inheritdoc />
	public SienarStatusService(
		ILogger<StatusService<TRequest>> logger,
		IEnumerable<IAccessValidator<TRequest>> accessValidators,
		IEnumerable<IStateValidator<TRequest>> stateValidators,
		IEnumerable<IBeforeProcess<TRequest>> beforeHooks,
		IEnumerable<IAfterProcess<TRequest>> afterHooks,
		IProcessor<TRequest, bool> processor,
		INotificationService notifier,
		IBotDetector botDetector)
		: base(
			logger,
			accessValidators,
			stateValidators,
			beforeHooks,
			afterHooks, 
			processor,
			notifier)
	{
		_botDetector = botDetector;
	}

	/// <inheritdoc />
	public override Task<bool> Execute(TRequest request)
	{
		if (request is Honeypot honeypot && _botDetector.IsSpambot(honeypot))
		{
			// Silently short-circuit spambots
			return Task.FromResult(true);
		}

		return base.Execute(request);
	}
}