using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Infrastructure.Services;

public class HookableService<TRequest> : IHookableService<TRequest>
{
	protected readonly ILogger<HookableService<TRequest>> Logger;
	protected readonly IEnumerable<IBeforeProcess<TRequest>> BeforeHooks;
	protected readonly IEnumerable<IAfterProcess<TRequest>> AfterHooks;
	protected readonly IProcessor<TRequest> Processor;

	public HookableService(
		ILogger<HookableService<TRequest>> logger,
		IEnumerable<IBeforeProcess<TRequest>> beforeHooks,
		IEnumerable<IAfterProcess<TRequest>> afterHooks,
		IProcessor<TRequest> processor)
	{
		Logger = logger;
		BeforeHooks = beforeHooks;
		AfterHooks = afterHooks;
		Processor = processor;
	}

	/// <inheritdoc />
	public virtual async Task<bool> Execute(TRequest request)
	{
		if (!await RunBeforeHooks(request))
		{
			Processor.NotifyBeforeHookFailure();
			return false;
		}

		try
		{
			if (await Processor.Process(request) != HookStatus.Success)
			{
				// Don't notify failure because the failure was calculated
				// so the IProcessor should notify the user
				return false;
			}
		}
		catch (Exception e)
		{
			Logger.LogError(e, "{type} failed to process", typeof(IProcessor<TRequest>));

			// Notify failure because the failure was unplanned
			Processor.NotifyProcessFailure();
			return false;
		}

		if (!await RunAfterHooks(request))
		{
			Processor.NotifyAfterHookFailure();
		}

		Processor.NotifySuccess();
		return true;
	}

	protected async Task<bool> RunBeforeHooks(TRequest model)
	{
		var successful = true;

		try
		{
			foreach (var hook in BeforeHooks)
			{
				if (await hook.Handle(model) != HookStatus.Success) successful = false;
			}
		}
		catch (Exception e)
		{
			Logger.LogError(e, "One or more before hooks failed to run");
			successful = false;
		}

		return successful;
	}

	protected async Task<bool> RunAfterHooks(TRequest model)
	{
		try
		{
			foreach (var hook in AfterHooks)
			{
				await hook.Handle(model);
			}
		}
		catch (Exception e)
		{
			Logger.LogError(e, "One or more after hooks failed to run");
			return false;
		}

		return true;
	}
}