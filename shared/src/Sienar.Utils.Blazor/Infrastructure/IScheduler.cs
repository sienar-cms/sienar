using System;

namespace Sienar.Infrastructure;

/// <summary>
/// Handles time-delayed method calls similarly to JavaScript's <c>setTimeout()</c> and <c>setInterval()</c>
/// </summary>
public interface IScheduler
{
	/// <summary>
	/// Registers an <see cref="Action"/> to be called once at a certain point in the future. Semantically identical to JavaScript's <c>setTimeout()</c> function 
	/// </summary>
	/// <param name="func">The action to call in the future</param>
	/// <param name="interval">The time interval to wait to call the action (in ms)</param>
	/// <returns>The ID of the timeout, which can be used to cancel the timeout by passing it to <see cref="ClearTimeout"/></returns>
	Guid SetTimeout(Action func, int interval);

	/// <summary>
	/// Clears a timeout that was previously registered. Semantically identical to JavaScript's <c>clearTimeout()</c> function
	/// </summary>
	/// <remarks>
	/// Similarly to the JavaScript <c>clearTimeout()</c> function, this method will silently fail if a timeout does not exist.
	/// </remarks>
	/// <param name="id">The ID of the timeout to clear</param>
	void ClearTimeout(Guid id);

	/// <summary>
	/// Registers an <see cref="Action"/> to be called repeatedly at certain intervals. Semantically identical to JavaScript's <c>setInterval()</c> function 
	/// </summary>
	/// <param name="func">The action to call</param>
	/// <param name="interval">The time interval to wait between calls (in ms)</param>
	/// <returns>The ID of the interval, which can be used to cancel the interval by passing it to <see cref="ClearInterval"/> method</returns>
	Guid SetInterval(Action func, int interval);

	/// <summary>
	/// Clears an interval that was previously registered. Semantically identical to JavaScript's <c>clearInterval()</c> function
	/// </summary>
	/// <remarks>
	/// Similarly to the JavaScript <c>clearInterval()</c> function, this method will silently fail if a timeout does not exist.
	/// </remarks>
	/// <param name="id">The ID of the interval to clear</param>
	void ClearInterval(Guid id);
}
