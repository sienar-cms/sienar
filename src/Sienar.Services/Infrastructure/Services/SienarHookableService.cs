using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Infrastructure.Services;

public class SienarHookableService<TRequest> : HookableService<TRequest>
{
	private readonly IBotDetector _botDetector;

	/// <inheritdoc />
	public SienarHookableService(
		ILogger<HookableService<TRequest>> logger,
		IEnumerable<IBeforeProcess<TRequest>> beforeHooks,
		IEnumerable<IAfterProcess<TRequest>> afterHooks,
		IProcessor<TRequest> processor,
		IBotDetector botDetector)
		: base(
			logger,
			beforeHooks,
			afterHooks, 
			processor)
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