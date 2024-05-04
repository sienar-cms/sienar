using System.Collections.Generic;
using Sienar.Infrastructure.Data;

namespace Sienar.Infrastructure;

public interface IReadableNotificationService : INotificationService
{
	/// <summary>
	/// The list of notifications registered in the notification service
	/// </summary>
	List<Notification> Notifications { get; }
}