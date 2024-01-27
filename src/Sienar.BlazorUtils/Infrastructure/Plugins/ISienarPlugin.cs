using Microsoft.AspNetCore.Builder;

namespace Sienar.Infrastructure.Plugins;

public interface ISienarPlugin
{
	void SetupDependencies(WebApplicationBuilder builder);

	void SetupApp(WebApplication app);

	PluginData PluginData { get; }

	PluginSettings PluginSettings { get; }
}