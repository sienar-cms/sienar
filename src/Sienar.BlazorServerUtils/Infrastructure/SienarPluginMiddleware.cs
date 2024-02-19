using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Infrastructure;

public class SienarPluginMiddleware
{
	private readonly RequestDelegate _next;

	public SienarPluginMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(
		HttpContext ctx,
		IEnumerable<ISienarPlugin> plugins,
		IStyleProvider styleProvider,
		IScriptProvider scriptProvider,
		IPluginProvider pluginProvider,
		IRoutableAssemblyProvider routableAssemblyProvider,
		IComponentProvider componentProvider,
		IDashboardProvider dashboardProvider,
		IMenuProvider menuProvider)
	{
		foreach (var plugin in plugins)
		{
			if (plugin.ShouldExecute())
			{
				pluginProvider.Add(plugin);
				plugin.Execute();
			}
		}

		await _next(ctx);
	}
}