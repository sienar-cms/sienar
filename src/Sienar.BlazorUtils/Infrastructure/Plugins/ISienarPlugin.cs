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
	/// <remarks>
	/// This property is an instance property because in <c>Sienar.Blazor/Pages/_Host.cshtml</c>, it is accessed via an instance. Therefore, this property can't be a <c>static virtual</c> member, so stop trying, Christian!
	/// </remarks>
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