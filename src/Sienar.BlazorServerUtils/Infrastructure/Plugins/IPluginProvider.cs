using System.Collections.Generic;

namespace Sienar.Infrastructure.Plugins;

public interface IPluginProvider
{
	/// <summary>
	/// Adds a plugin to the current plugin provider
	/// </summary>
	/// <param name="plugin">The plugin to add</param>
	/// <returns>this</returns>
	IPluginProvider Add(ISienarPlugin plugin);

	/// <summary>
	/// Gets all plugins in the plugin provider
	/// </summary>
	/// <returns>the list of plugins</returns>
	List<ISienarPlugin> GetPlugins();
}