using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Infrastructure.Services;

public class SienarService<TRequest, TResult> : Service<TRequest, TResult>
{
	private readonly IBotDetector _botDetector;

	/// <inheritdoc />
	public SienarService(
		ILogger<Service<TRequest, TResult>> logger,
		IEnumerable<IAccessValidator<TRequest>> accessValidators,
		IEnumerable<IStateValidator<TRequest>> stateValidators,
		IEnumerable<IBeforeProcess<TRequest>> beforeHooks,
		IEnumerable<IAfterProcess<TRequest>> afterHooks,
		IProcessor<TRequest, TResult> processor,
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
	public override Task<TResult?> Execute(TRequest request)
	{
		if (request is Honeypot honeypot && _botDetector.IsSpambot(honeypot))
		{
			return Task.FromResult(default(TResult));
		}

		return base.Execute(request);
	}
}