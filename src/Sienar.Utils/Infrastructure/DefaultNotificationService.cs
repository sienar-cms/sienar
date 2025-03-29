using System.Collections.Generic;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <summary>
/// The default implementation of <see cref="INotificationService"/>
/// </summary>
public class DefaultNotificationService : INotificationService
{
	/// <inheritdoc />
	public List<Notification> Notifications { get; } = [];

	/// <inheritdoc />
	public void Success(string message)
		=> Notifications.Add(new(message, NotificationType.Success));

	/// <inheritdoc />
	public void Warning(string message)
		=> Notifications.Add(new(message, NotificationType.Warning));

	/// <inheritdoc />
	public void Info(string message)
		=> Notifications.Add(new(message, NotificationType.Info));

	/// <inheritdoc />
	public void Error(string message)
		=> Notifications.Add(new(message, NotificationType.Error));

	/// <inheritdoc />
	public void Notify(Notification notification)
		=> Notifications.Add(notification);
}