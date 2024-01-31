using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Sienar.Infrastructure.Menus;

namespace Sienar.Infrastructure.Plugins;

public interface ISienarPlugin
{
	PluginData PluginData { get; }

	PluginSettings PluginSettings { get; }

	void SetupDependencies(WebApplicationBuilder builder);

	bool PluginShouldExecute(HttpContext context);

	void SetupApp(WebApplication app);

	void SetupMenu(IMenuProvider menuProvider);

	void SetupStyles(IStyleProvider styleProvider);

	void SetupComponents(IComponentProvider componentProvider);

	void SetupScripts(IScriptProvider scriptProvider);
}