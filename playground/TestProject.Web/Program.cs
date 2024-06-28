using Microsoft.Extensions.DependencyInjection;
using Sienar.Email;
using Sienar.Extensions;
using Sienar.Infrastructure.Plugins;
using Sienar.Infrastructure;
using TestProject.Web.Data;
using TestProject.Web.Extensions;
using TestProject.Web.Layouts;
using TestProject.Web.UI;

await SienarWebAppBuilder
	.Create(args, typeof(Program).Assembly)
	.AddRootDbContext<AppDbContext>(o => o.UseSienarDb())
	.AddPlugin<SienarCmsBlazorPlugin>()
	.AddPlugin<MailKitPlugin>()
#if DEBUG
	.AddPlugin<DevmodePlugin>()
#endif
	.AddPlugin<SienarBlazorPlugin>()
	.SetupApp(
		app =>
		{
			app.Services.MigrateDb<AppDbContext>(SienarDataExtensions.GetSienarDbPath());
			app.Services
				.ConfigureComponents(
					p =>
					{
						p.DefaultLayout = typeof(MainAppLayout);
						p.AppbarLeft = typeof(Branding);
					})
				.ConfigureMenu(p => p.AddMenu())
				.ConfigureStyles(p =>
				{
					p.Add("/styles.css");
					p.Add("/TestProject.Web.styles.css");
				});
		})
	.ConfigureTheme<SienarTheme>()
	.Build()
	.RunAsync();
