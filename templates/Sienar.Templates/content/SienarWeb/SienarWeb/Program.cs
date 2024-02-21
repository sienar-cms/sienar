using Microsoft.Extensions.DependencyInjection;
using Sienar.Email;
using Sienar.Infrastructure.Plugins;
using Sienar.Infrastructure;
using SienarWeb;
using SienarWeb.Data;

await SienarServerAppBuilder
	.Create<AppDbContext>(
		args,
		o => o.UseDb(),
		dbContextLifetime: ServiceLifetime.Transient)
	.AddPlugin<SienarBlazorPlugin>()
	.AddPlugin<SienarCmsPlugin>()
	.AddStartupPlugin<MailKitPlugin>()
	.AddPlugin<SienarWebPlugin>()
	.ConfigureTheme<CustomTheme>()
	.Build()
	.RunAsync();