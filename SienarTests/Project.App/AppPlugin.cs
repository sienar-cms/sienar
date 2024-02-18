using System;
using Sienar.Infrastructure.Plugins;

namespace Project.App;

public class AppPlugin : ISienarPlugin
{
	private readonly IPluginExecutionTracker _executionTracker;

	public AppPlugin(IPluginExecutionTracker executionTracker)
	{
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
	public bool ShouldExecute()
	{
		if (!_executionTracker.SubAppHasExecuted)
		{
			_executionTracker.ClaimExecution();
			return true;
		}

		return false;
	}

	/// <inheritdoc />
	public void SetupStyles(IStyleProvider styleProvider)
		=> styleProvider.Add("/css/site.css");

	/// <inheritdoc />
	public void SetupRoutableAssemblies(
		IRoutableAssemblyProvider routableAssemblyProvider)
		=> routableAssemblyProvider.Add(typeof(AppPlugin).Assembly);
}