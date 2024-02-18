using System;
using Sienar.Infrastructure.Menus;

namespace Sienar.Infrastructure.Plugins;

public interface ISienarPlugin
{
	/// <summary>
	/// Provides the <see cref="Type"/> of the startup plugin to use for this plugin, if any
	/// </summary>
	static virtual Type? StartupPlugin => null;

	/// <summary>
	/// Plugin data for the current plugin
	/// </summary>
	PluginData PluginData { get; }

	/// <summary>
	/// Configures various components to replace specific parts of the Sienar UI
	/// </summary>
	/// <param name="componentProvider">the <see cref="IComponentProvider"/></param>
	void SetupComponents(IComponentProvider componentProvider) {}

	/// <summary>
	/// Configures dashboard items to be registered for the current user session
	/// </summary>
	/// <param name="dashboardProvider">the <see cref="IDashboardProvider"/> containing dashboard item definitions</param>
	void SetupDashboard(IDashboardProvider dashboardProvider) {}

	/// <summary>
	/// Configures menu items to be registered for the current user session
	/// </summary>
	/// <param name="menuProvider">the <see cref="IMenuProvider"/> containing menu item definitions</param>
	void SetupMenu(IMenuProvider menuProvider) {}

	/// <summary>
	/// Configures routable assemblies
	/// </summary>
	/// <param name="routableAssemblyProvider">the <see cref="IRoutableAssemblyProvider"/></param>
	void SetupRoutableAssemblies(IRoutableAssemblyProvider routableAssemblyProvider) {}

	/// <summary>
	/// Configures scripts to be loaded with the current user session
	/// </summary>
	/// <param name="scriptProvider">the <see cref="IScriptProvider"/></param>
	void SetupScripts(IScriptProvider scriptProvider) {}

	/// <summary>
	/// Configures stylesheets to be loaded with the current user session
	/// </summary>
	/// <param name="styleProvider">the <see cref="IStyleProvider"/></param>
	void SetupStyles(IStyleProvider styleProvider) {}

	/// <summary>
	/// Determines whether the plugin should execute on the current request
	/// </summary>
	/// <returns>whether the plugin should execute</returns>
	bool ShouldExecute() => true;
}