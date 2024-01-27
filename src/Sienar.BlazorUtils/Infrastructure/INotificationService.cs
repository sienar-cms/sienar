using System;

namespace Sienar.Infrastructure;

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
	/// Used to display an error message to the user
	/// </summary>
	/// <param name="e">The exception that triggered the error message</param>
	void Error(Exception e);
}