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
		IServiceProvider serviceProvider)
	{
		foreach (var plugin in plugins)
		{
			if (plugin.ShouldExecute())
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