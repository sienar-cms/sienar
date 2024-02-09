namespace Sienar.Infrastructure.Plugins;

public interface IPluginExecutionTracker
{
	/// <summary>
	/// Indicates whether another subapp-level plugin has executed
	/// </summary>
	bool SubAppHasExecuted { get; }

	/// <summary>
	/// Informs the plugin execution tracker that a subapp-level plugin has executed
	/// </summary>
	void ClaimExecution();
}