using MudBlazor;
using Sienar.Extensions;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Plugins;

await SienarClientAppBuilder
	.Create(args)
	.AddPlugin<SienarDocsPlugin>()
	.ConfigureTheme(new MudTheme())
	.Build()
	.RunAsync();