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
	.AddPlugin(new SienarBlazorPlugin())
	.AddPlugin(new SienarCmsPlugin())
	.AddPlugin(new MailKitPlugin())
	.AddPlugin(new AppPlugin())
	.ConfigureTheme(new CustomTheme())
	.Build()
	.RunAsync();
