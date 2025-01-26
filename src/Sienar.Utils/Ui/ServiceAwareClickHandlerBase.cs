using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Sienar.Ui;

/// <summary>
/// A base class for handling <c>onclick</c> events using a handler which injects services in its callback signature
/// </summary>
public class ServiceAwareClickHandlerBase : ComponentBase
{
	/// <summary>
	/// The application's service provider
	/// </summary>
	[Inject]
	protected IServiceProvider Sp { get; set; } = null!;

	/// <summary>
	/// Handles clicks by invoking a <c>delegate</c> with a dynamic signature whose arguments are supplied from the DI container
	/// </summary>
	/// <remarks>
	/// The supplied <c>delegate</c> can have any argument at any position, as long as that argument can be resolved from the DI container. The only exception is if the <c>delegate</c> has an argument typed <see cref="MouseEventArgs"/>, in which case it will receive the copy provided by the framework in response to the click event (this argument can still be in any position).
	/// </remarks>
	/// <param name="handler">The <c>onclick</c> delegate. If <c>null</c>, this method returns early</param>
	/// <param name="e">The <see cref="MouseEventArgs"/> supplied by Blazor in response to a click event</param>
	protected void HandleClick(Delegate? handler, MouseEventArgs e)
	{
		if (handler is null) return;

		var parameters = handler.Method.GetParameters();
		var args = new object?[parameters.Length];

		for (var i = 0; i < parameters.Length; i++)
		{
			var parameterType = parameters[i].ParameterType;

			if (parameterType == typeof(MouseEventArgs))
			{
				args[i] = e;
			}
			else
			{
				args[i] = Sp.GetRequiredService(parameterType);
			}
		}

		handler.DynamicInvoke(args);
	}
}
