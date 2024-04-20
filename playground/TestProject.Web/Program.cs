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
			app.ConfigureStyles(p =>
			{
				p.Add("/styles.css");
				p.Add("/TestProject.Web.styles.css");
			});
		})
	.ConfigureTheme<SienarTheme>()
	.BuildBlazor()
	.RunAsync();
