using System;
using System.Collections.Generic;
using System.Timers;

namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultScheduler : IScheduler
{
	private readonly Dictionary<Guid, ScheduledTask> _tasks = new();
	private readonly IDelegateHandler _delegateHandler;

	/// <summary>
	/// Creates a new instance of <c>DefaultScheduler</c>
	/// </summary>
	/// <param name="delegateHandler">The delegate handler</param>
	public DefaultScheduler(IDelegateHandler delegateHandler) => _delegateHandler = delegateHandler;

	/// <inheritdoc />
	public Guid SetTimeout(Delegate func, int interval)
		=> CreateTask(func, interval, false);

	/// <inheritdoc />
	public void ClearTimeout(Guid id) => DeleteTask(id);

	/// <inheritdoc />
	public Guid SetInterval(Delegate func, int interval)
		=> CreateTask(func, interval, true);

	/// <inheritdoc />
	public void ClearInterval(Guid id) => DeleteTask(id);

	private Guid CreateTask(Delegate func, int interval, bool shouldRepeat)
	{
		var id = Guid.NewGuid();
		ElapsedEventHandler handler = (_, e) =>
		{
			_delegateHandler.Handle(func, e);

			if (!shouldRepeat)
			{
				ClearTimeout(id);
			}
		};
		var timer = new Timer(interval)
		{
			Enabled = true,
			AutoReset = shouldRepeat
		};
		timer.Elapsed += handler;

		var task = new ScheduledTask
		{
			Action = func,
			Timer = timer,
			Handler = handler
		};

		_tasks[id] = task;
		return id;
	}

	private void DeleteTask(Guid id)
	{
		// For parity with the JavaScript clearTimeout()/clearInterval() API,
		// we should silently return if the timeout doesn't exist
		if (!_tasks.TryGetValue(id, out var timeout))
		{
			return;
		}

		timeout.Timer.Elapsed -= timeout.Handler;
		timeout.Timer.Dispose();
		_tasks.Remove(id);
	}

	private class ScheduledTask
	{
		public required Delegate Action { get; set; }
		public required ElapsedEventHandler Handler { get; set; }
		public required Timer Timer { get; set; }
	}
}
