using System;
using System.Collections.Generic;
using Sienar.Exceptions;
using Sienar.Infrastructure.Data;

namespace Sienar.Infrastructure;

public class RestNotificationService : IReadableNotificationService
{
	/// <inheritdoc />
	public List<Notification> Notifications { get; } = [];

	/// <inheritdoc />
	public void Success(string message)
		=> EnqueueNotification(message, NotificationType.Success);

	/// <inheritdoc />
	public void Warning(string message)
		=> EnqueueNotification(message, NotificationType.Warning);

	/// <inheritdoc />
	public void Info(string message)
		=> EnqueueNotification(message, NotificationType.Info);

	/// <inheritdoc />
	public void Error(string message)
		=> EnqueueNotification(message, NotificationType.Error);

	/// <inheritdoc />
	public void Error(Exception e)
	{
		var message = e is SienarException
			? e.Message
			: StatusMessages.General.Unknown;
		EnqueueNotification(message, NotificationType.Error);
	}

	private void EnqueueNotification(string message, NotificationType type)
	{
		Notifications.Add(new(message, type));
	}
}