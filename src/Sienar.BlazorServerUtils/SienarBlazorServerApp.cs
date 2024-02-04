using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;

namespace Sienar;

public class SienarBlazorServerApp : SienarApp
{
	[Parameter]
	public string[] PluginNames { get; set; } = Array.Empty<string>();

	[Inject]
	protected IPluginProvider PluginProvider { get; set; } = default!;

	[Inject(Key = SienarBlazorUtilsServiceKeys.MenuProvider)]
	protected IMenuProvider MenuProvider { get; set; } = default!;

	[Inject(Key = SienarBlazorUtilsServiceKeys.DashboardProvider)]
	protected IMenuProvider DashboardProvider { get; set; } = default!;

	[Inject]
	protected IEnumerable<ISienarServerPlugin> SienarPlugins { get; set; } = Array.Empty<ISienarServerPlugin>();

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

		if (plugin.PluginSettings.HasRoutableComponents)
		{
			AssemblyProvider.Add(plugin.GetType().Assembly);
		}

		if (plugin.PluginSettings.UsesProviders)
		{
			plugin.SetupMenu(MenuProvider);
			plugin.SetupDashboard(DashboardProvider);
			plugin.SetupComponents(ComponentProvider);
		}
	}
}