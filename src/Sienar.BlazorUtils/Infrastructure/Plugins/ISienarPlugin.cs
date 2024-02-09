using Sienar.Infrastructure.Menus;

namespace Sienar.Infrastructure.Plugins;

public interface ISienarPlugin
{
	/// <summary>
	/// Plugin data for the current plugin
	/// </summary>
	PluginData PluginData { get; }

	/// <summary>
	/// Configures stylesheets to be loaded with the current user session
	/// </summary>
	/// <param name="styleProvider">the <see cref="IStyleProvider"/></param>
	void SetupStyles(IStyleProvider styleProvider);

	/// <summary>
	/// Configures scripts to be loaded with the current user session
	/// </summary>
	/// <param name="scriptProvider">the <see cref="IScriptProvider"/></param>
	void SetupScripts(IScriptProvider scriptProvider);

	/// <summary>
	/// Configures menu items to be registered for the current user session
	/// </summary>
	/// <param name="menuProvider">the <see cref="IMenuProvider"/> containing menu item definitions</param>
	void SetupMenu(IMenuProvider menuProvider);

	/// <summary>
	/// Configures dashboard items to be registered for the current user session
	/// </summary>
	/// <param name="dashboardProvider">the <see cref="IMenuProvider"/> containing dashboard item definitions</param>
	void SetupDashboard(IMenuProvider dashboardProvider);

	/// <summary>
	/// Configures various components to replace specific parts of the Sienar UI
	/// </summary>
	/// <param name="componentProvider">the <see cref="IComponentProvider"/></param>
	void SetupComponents(IComponentProvider componentProvider);

	/// <summary>
	/// Configures routable assemblies
	/// </summary>
	/// <param name="routableAssemblyProvider">the <see cref="IRoutableAssemblyProvider"/></param>
	void SetupRoutableAssemblies(IRoutableAssemblyProvider routableAssemblyProvider);
}