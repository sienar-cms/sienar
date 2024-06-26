using Sevita.Tools;
using Sevita.Tools.Data;
using Sevita.Tools.Extensions;
using Sienar.Email;
using Sienar.Extensions;
using Sienar.Infrastructure.Plugins;
using Sienar.Infrastructure;

await SienarWebAppBuilder
	.Create(args, typeof(Program).Assembly)
	.AddRootDbContext<AppDbContext>(o => o.UseSevitaDb())
	.AddPlugin<SienarCmsBlazorPlugin>()
	.AddPlugin<MailKitPlugin>()
#if DEBUG
	.AddPlugin<DevmodePlugin>()
#endif
	.SetupDependencies(builder => builder.SettupSevitaToolsDependencies())
	.SetupApp(app => app.SetupSevitaTools())
	.ConfigureTheme<SevitaTheme>()
	.AddPlugin<SienarBlazorPlugin>() // Must be last plugin registered
	.Build()
	.RunAsync();
