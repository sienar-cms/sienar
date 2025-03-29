using System;
using System.Collections.Generic;
using Sienar.Data;

namespace Sienar.Infrastructure;

/// <summary>
/// Supplies various types of messages to the user
/// </summary>
public interface INotificationService
{
	/// <summary>
	/// Used to display a success message to the user
	/// </summary>
	/// <param name="message">The message to display</param>
	void Success(string message);

	/// <summary>
	/// Used to display a warning message to the user
	/// </summary>
	/// <param name="message">The message to display</param>
	void Warning(string message);

	/// <summary>
	/// Used to display an informational message to the user
	/// </summary>
	/// <param name="message">The message to display</param>
	void Info(string message);

	/// <summary>
	/// Used to display an error message to the user
	/// </summary>
	/// <param name="message">The message to display</param>
	void Error(string message);

	/// <summary>
	/// Used to display an arbitrary notification to the user
	/// </summary>
	/// <param name="notification">The notification to display</param>
	void Notify(Notification notification);

	/// <summary>
	/// The list of notifications registered in the notification service
	/// </summary>
	List<Notification> Notifications { get; }
}