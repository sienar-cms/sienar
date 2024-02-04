using Sienar.Infrastructure.Menus;

namespace Sienar.Infrastructure.Plugins;

public interface ISienarPlugin
{
	PluginData PluginData { get; }

	PluginSettings PluginSettings { get; }

	void SetupStyles(IStyleProvider styleProvider);

	void SetupScripts(IScriptProvider scriptProvider);

	void SetupMenu(IMenuProvider menuProvider);

	void SetupDashboard(IMenuProvider dashboardProvider);

	void SetupComponents(IComponentProvider componentProvider);
}