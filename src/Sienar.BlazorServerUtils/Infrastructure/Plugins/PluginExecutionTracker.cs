namespace Sienar.Infrastructure.Plugins;

public class PluginExecutionTracker : IPluginExecutionTracker
{
	/// <inheritdoc />
	public bool SubAppHasExecuted { get; private set; }

	/// <inheritdoc />
	public void ClaimExecution() => SubAppHasExecuted = true;
}