using Microsoft.Extensions.DependencyInjection;
using Project.App;
using Project.Data;
using Sienar.Email;
using Sienar.Infrastructure.Plugins;
using Sienar.Infrastructure;

await SienarServerAppBuilder
	.Create<AppDbContext>(
		args,
		o => o.UseSienarDb(),
		ServiceLifetime.Transient)
	.AddPlugin<SienarBlazorPlugin>()
	.AddPlugin<SienarCmsPlugin>()
	.AddStartupPlugin<MailKitPlugin>()
	.AddPlugin<AppPlugin>()
	.ConfigureTheme<CustomTheme>()
	.Build()
	.RunAsync();
