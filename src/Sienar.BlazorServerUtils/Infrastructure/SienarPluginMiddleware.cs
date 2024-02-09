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
	private readonly IEnumerable<ISienarServerPlugin> _plugins;

	public SienarPluginMiddleware(
		RequestDelegate next,
		IEnumerable<ISienarServerPlugin> plugins)
	{
		_next = next;
		_plugins = plugins;
	}

	public async Task InvokeAsync(
		HttpContext ctx,
		IPluginExecutionTracker executionTracker,
		IStyleProvider styleProvider,
		IScriptProvider scriptProvider,
		IPluginProvider pluginProvider,
		IRoutableAssemblyProvider routableAssemblyProvider,
		IComponentProvider componentProvider,
		IServiceProvider serviceProvider)
	{
		foreach (var plugin in _plugins)
		{
			if (plugin.PluginShouldExecute(ctx, executionTracker))
			{
				pluginProvider.Add(plugin);

				var menuProvider = serviceProvider.GetRequiredKeyedService<IMenuProvider>(
					SienarBlazorUtilsServiceKeys.MenuProvider);
				var dashboardProvider = serviceProvider.GetRequiredKeyedService<IMenuProvider>(
					SienarBlazorUtilsServiceKeys.DashboardProvider);

				plugin.SetupStyles(styleProvider);
				plugin.SetupScripts(scriptProvider);
				plugin.SetupMenu(menuProvider);
				plugin.SetupDashboard(dashboardProvider);
				plugin.SetupComponents(componentProvider);
				plugin.SetupRoutableAssemblies(routableAssemblyProvider);
			}
		}

		await _next(ctx);
	}
}