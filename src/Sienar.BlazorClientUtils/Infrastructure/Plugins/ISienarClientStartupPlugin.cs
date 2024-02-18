using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Sienar.Infrastructure.Plugins;

public interface ISienarClientStartupPlugin
{
	void SetupDependencies(WebAssemblyHostBuilder builder) {}

	void SetupApp(WebAssemblyHost app) {}
}