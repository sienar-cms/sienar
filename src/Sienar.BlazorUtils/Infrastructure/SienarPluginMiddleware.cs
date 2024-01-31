using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
		IPluginProvider pluginProvider)
	{
		if (_plugin.PluginShouldExecute(ctx))
		{
			pluginProvider.Add(_plugin);

			if (_plugin.PluginSettings.ModifiesStyles)
			{
				_plugin.SetupStyles(styleProvider);
			}

			if (_plugin.PluginSettings.ModifiesScripts)
			{
				_plugin.SetupScripts(scriptProvider);
			}
		}

		await _next(ctx);
	}
}