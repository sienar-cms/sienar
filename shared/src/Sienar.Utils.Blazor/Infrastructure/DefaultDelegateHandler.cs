using System;
using Microsoft.Extensions.DependencyInjection;

namespace Sienar.Infrastructure;

/// <summary>
/// Calls Blazor event handlers in a way that allows method arguments to be supplied from the DI container 
/// </summary>
public class DefaultDelegateHandler : IDelegateHandler
{
	private readonly IServiceProvider _sp;

	/// <summary>
	/// Instantiates a new instance of <c>DefaultDelegateHandler</c>
	/// </summary>
	/// <param name="sp">The current service provider</param>
	public DefaultDelegateHandler(IServiceProvider sp)
	{
		_sp = sp;
	}

	/// <inheritdoc />
	public void Handle<TArgs>(Delegate? handler, TArgs e)
	{
		if (handler is null)
		{
			return;
		}

		var parameters = handler.Method.GetParameters();
		var args = new object?[parameters.Length];

		for (var i = 0; i < parameters.Length; i++)
		{
			var parameterType = parameters[i].ParameterType;

			if (parameterType == typeof(TArgs))
			{
				args[i] = e;
			}
			else
			{
				args[i] = _sp.GetRequiredService(parameterType);
			}
		}

		handler.DynamicInvoke(args);
	}
}
