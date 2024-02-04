using System.Collections.Generic;

namespace Sienar.Infrastructure.Plugins;

public class PluginProvider : IPluginProvider
{
	private readonly List<ISienarServerPlugin> _plugins = [];

	/// <inheritdoc />
	public IPluginProvider Add(ISienarServerPlugin plugin)
	{
		_plugins.Add(plugin);
		return this;
	}

	/// <inheritdoc />
	public List<ISienarServerPlugin> GetPlugins() => _plugins;
}