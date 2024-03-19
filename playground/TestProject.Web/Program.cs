using Microsoft.Extensions.DependencyInjection;
using Sienar.Email;
using Sienar.Extensions;
using Sienar.Infrastructure.Plugins;
using Sienar.Infrastructure;
using TestProject.Web;
using TestProject.Web.Data;
using TestProject.Web.Extensions;
using TestProject.Web.Layouts;
using TestProject.Web.UI;

await SienarAppBuilder
	.Create(args, typeof(Program).Assembly)
	.AddRootDbContext<AppDbContext>(o => o.UseSienarDb())
	.AddPlugin<SienarCmsPlugin>()
	.AddPlugin<MailKitPlugin>()
#if DEBUG
	.AddPlugin<DevmodePlugin>()
#endif
	.SetupDependencies(
		builder => builder.Services.AddRequestConfigurer<AppRequestConfigurer>())
	.SetupApp(
		app =>
		{
			app.Services.MigrateDb<AppDbContext>(SienarDataExtensions.GetSienarDbPath());
			app.ConfigureComponents(
				p =>
				{
					p.DefaultLayout = typeof(MainAppLayout);
					p.AppbarLeft = typeof(Branding);
				});
			app.ConfigureMenu(p => p.AddMenu());
		})
	.ConfigureTheme<SienarTheme>()
	.BuildBlazor()
	.RunAsync();
