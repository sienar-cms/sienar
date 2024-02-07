using MudBlazor;
using Sienar.Extensions;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Plugins;

await SienarClientAppBuilder
	.Create(args)
	.AddPlugin(new SienarDocsPlugin())
	.ConfigureTheme(new MudTheme())
	.Build()
	.RunAsync();