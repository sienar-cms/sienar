using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Sienar.Infrastructure.Plugins;

public interface ISienarClientPlugin : ISienarPlugin
{
	void SetupDependencies(WebAssemblyHostBuilder builder);

	void SetupApp(WebAssemblyHost app);

	void SetupRootComponents(IRootComponentProvider rootComponentProvider);
}