using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Infrastructure;

public class SienarPluginMiddleware<TPlugin>
	where TPlugin : ISienarPlugin
{
	private readonly RequestDelegate _next;
	private readonly TPlugin _plugin;

	public SienarPluginMiddleware(
		RequestDelegate next,
		TPlugin plugin)
	{
		_next = next;
		_plugin = plugin;
	}

	public async Task InvokeAsync(
		HttpContext ctx,
		IStyleProvider styleProvider,
		IScriptProvider scriptProvider,
		IPluginProvider pluginProvider,
		IMenuProvider menuProvider,
		IRoutableAssemblyProvider routableAssemblyProvider,
		IComponentProvider componentProvider)
	{
		if (_plugin.PluginShouldExecute(ctx))
		{
			pluginProvider.Add(_plugin);

			if (_plugin.PluginSettings.HasRoutableComponents)
			{
				routableAssemblyProvider.Add(_plugin.GetType().Assembly);
			}

			_plugin.SetupStyles(styleProvider);
			_plugin.SetupScripts(scriptProvider);
			_plugin.SetupMenu(menuProvider);
			_plugin.SetupComponents(componentProvider);
		}

		await _next(ctx);
	}
}