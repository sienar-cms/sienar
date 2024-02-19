using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Sienar.Infrastructure.Plugins;

namespace Sienar;

public class SienarBlazorServerApp : SienarApp
{
	[Parameter]
	public string[] PluginNames { get; set; } = Array.Empty<string>();

	[Inject]
	protected IPluginProvider PluginProvider { get; set; } = default!;

	[Inject]
	protected IEnumerable<ISienarPlugin> SienarPlugins { get; set; } = Array.Empty<ISienarPlugin>();

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		if (PluginProvider.GetPlugins().Count == 0)
		{
			foreach (var pluginName in PluginNames)
			{
				SetupPlugin(pluginName);
			}
		}

		base.OnInitialized();
	}

	protected void SetupPlugin(string name)
	{
		var plugin = SienarPlugins.First(p => p.PluginData.Name == name);
		PluginProvider.Add(plugin);
		plugin.Execute();
	}
}