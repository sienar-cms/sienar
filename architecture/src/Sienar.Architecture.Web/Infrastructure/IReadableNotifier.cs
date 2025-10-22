using System.Collections.Generic;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <summary>
/// A version of <see cref="INotifier"/> that allows developers to retrieve the underlying notifications so they can be shown to users
/// </summary>
public interface IReadableNotifier : INotifier
{
	/// <summary>
	/// The list of notifications registered in the notification service
	/// </summary>
	List<Notification> Notifications { get; }
}