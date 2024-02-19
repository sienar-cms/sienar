using System;

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
	/// Executes a plugin for the current request
	/// </summary>
	void Execute();

	/// <summary>
	/// Determines whether the plugin should execute on the current request
	/// </summary>
	/// <returns>whether the plugin should execute</returns>
	bool ShouldExecute() => true;
}