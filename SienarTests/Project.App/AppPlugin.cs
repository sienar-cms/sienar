using System;
using Sienar.Infrastructure.Plugins;

namespace Project.App;

public class AppPlugin : ISienarPlugin
{
	private readonly IStyleProvider _styleProvider;
	private readonly IRoutableAssemblyProvider _routableAssemblyProvider;
	private readonly IPluginExecutionTracker _executionTracker;

	public AppPlugin(
		IStyleProvider styleProvider,
		IRoutableAssemblyProvider routableAssemblyProvider,
		IPluginExecutionTracker executionTracker)
	{
		_styleProvider = styleProvider;
		_routableAssemblyProvider = routableAssemblyProvider;
		_executionTracker = executionTracker;
	}

	/// <inheritdoc />
	public static Type StartupPlugin => typeof(AppStartupPlugin);

	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "App plugin",
		Author = string.Empty,
		AuthorUrl = string.Empty,
		Version = Version.Parse("1.0.0"),
		Description = string.Empty
	};

	/// <inheritdoc />
	public void Execute()
	{
		SetupStyles();
		SetupRoutableAssemblies();
	}

	/// <inheritdoc />
	public bool ShouldExecute()
	{
		if (!_executionTracker.SubAppHasExecuted)
		{
			_executionTracker.ClaimExecution();
			return true;
		}

		return false;
	}

	private void SetupStyles()
		=> _styleProvider.Add("/css/site.css");

	private void SetupRoutableAssemblies()
		=> _routableAssemblyProvider.Add(typeof(AppPlugin).Assembly);
}