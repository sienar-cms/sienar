using System.Collections.Generic;

namespace Sienar.Infrastructure.Plugins;

public class PluginProvider : IPluginProvider
{
	private readonly List<ISienarPlugin> _plugins = [];

	/// <inheritdoc />
	public IPluginProvider Add(ISienarPlugin plugin)
	{
		_plugins.Add(plugin);
		return this;
	}

	/// <inheritdoc />
	public List<ISienarPlugin> GetPlugins() => _plugins;
}