using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Sevita.Tools.Data;
using Sevita.Tools.UI;
using Sienar;
using Sienar.Extensions;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;

namespace Sevita.Tools.Extensions;

public static class SevitaToolsWebApplicationExtensions
{
	public static void SetupSevitaTools(this WebApplication app)
	{
		app.Services.MigrateDb<AppDbContext>(SienarDataExtensions.GetSevitaDbPath());
		app.ConfigureComponents(SetupComponents);
		app.ConfigureMenu(SetupMenu);
		app.ConfigureStyles(SetupStyles);
	}

	private static void SetupComponents(IComponentProvider p)
	{
		p.AppbarLeft = typeof(Branding);
	}

	private static void SetupMenu(IMenuProvider p)
	{
		p.Access(DashboardMenuNames.MainMenu)
			.AddLink(new()
			{
				Text = "Users",
				Icon = Icons.Material.Filled.SupervisorAccount,
				Roles = [Roles.Admin],
				Sublinks = [
					new()
					{
						Text = "Listing",
						Icon = Icons.Material.Filled.SupervisorAccount,
						Url = DashboardUrls.Users.Index
					},
					new()
					{
						Text = "Logout reasons",
						Icon = Icons.Material.Filled.Lock,
						Url = DashboardUrls.LockoutReasons.Index
					}
				]
			});
	}

	private static void SetupStyles(IStyleProvider p)
	{
		p.Add("/styles.css");
		p.Add("/Sevita.Tools.styles.css");
	}
}