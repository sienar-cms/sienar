using System;

namespace Sienar.Infrastructure;

/// <summary>
/// A handler for JavaScript events for use in Blazor-sourced events such as <c>@onclick</c>
/// </summary>
public interface IDelegateHandler
{
	/// <summary>
	/// Handles JavaScript events by invoking a <c>delegate</c> with a dynamic signature whose arguments are supplied from the DI container
	/// </summary>
	/// <remarks>
	/// The supplied <c>delegate</c> can have any argument at any position, as long as that argument type can be resolved from the DI container. The only exception is the CLR type of the event arguments (e.g., <see cref="Microsoft.AspNetCore.Components.Web.MouseEventArgs"/>), in which case it will receive the copy of the arguments provided by the framework in response to the event (this argument can still be in any position in the delegate signature).
	/// </remarks>
	/// <param name="handler">The event delegate</param>
	/// <param name="e">The event args supplied by Blazor in response to an event</param>
	/// <typeparam name="TArgs">The type of the event</typeparam>
	void Handle<TArgs>(Delegate? handler, TArgs e);
}
