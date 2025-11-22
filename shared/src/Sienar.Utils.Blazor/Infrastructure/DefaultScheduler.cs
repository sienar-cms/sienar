using System;
using System.Collections.Generic;
using System.Timers;

namespace Sienar.Infrastructure;

/// <inheritdoc />
public class DefaultScheduler : IScheduler
{
	private readonly Dictionary<Guid, ScheduledTask> _tasks = new();

	/// <inheritdoc />
	public Guid SetTimeout(Action func, int interval)
		=> CreateTask(func, interval, false);

	/// <inheritdoc />
	public void ClearTimeout(Guid id) => DeleteTask(id);

	/// <inheritdoc />
	public Guid SetInterval(Action func, int interval)
		=> CreateTask(func, interval, true);

	/// <inheritdoc />
	public void ClearInterval(Guid id) => DeleteTask(id);

	private Guid CreateTask(Action func, int interval, bool shouldRepeat)
	{
		var id = Guid.NewGuid();
		ElapsedEventHandler handler = (_, _) =>
		{
			func();

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
		public required Action Action { get; set; }
		public required ElapsedEventHandler Handler { get; set; }
		public required Timer Timer { get; set; }
	}
}
