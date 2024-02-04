using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Sienar.Infrastructure.Plugins;

public interface ISienarServerPlugin : ISienarPlugin
{
	void SetupDependencies(WebApplicationBuilder builder);

	bool PluginShouldExecute(HttpContext context);

	void SetupApp(WebApplication app);
}