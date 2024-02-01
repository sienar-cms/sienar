using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MudBlazor;
using Sienar.Infrastructure.Menus;
using Sienar.UI;

namespace Sienar.Infrastructure.Plugins;

public class SienarCmsPlugin : ISienarPlugin
{
	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "Sienar CMS",
		Version = Version.Parse("0.1.0"),
		Author = "Christian LeVesque",
		AuthorUrl = "https://levesque.dev",
		Description = "Sienar CMS provides pages and hooks that allow users to manage their account and media. Sienar itself can operate without this plugin, but core functionality like logging in won't work.",
		Homepage = "https://sienar.siteveyor.com"
	};

	/// <inheritdoc />
	public PluginSettings PluginSettings { get; } = new()
	{
		HasRoutableComponents = true,
		UsesProviders = true
	};

	/// <inheritdoc />
	public void SetupDependencies(WebApplicationBuilder builder)
	{
		builder.Services
			.AddIdentityHooks()
			.AddMediaHooks();
	}

	/// <inheritdoc />
	public void SetupApp(WebApplication app)
	{
		app.MapFallbackToPage("/dashboard/{**segment}", "/_Host");
	}

	public bool PluginShouldExecute(HttpContext context)
		=> context.Request.Path.StartsWithSegments("/dashboard");

	public void SetupMenu(IMenuProvider menuProvider)
	{
		CreateMainMenu(menuProvider);
		CreateInfoMenu(menuProvider);
	}

	/// <inheritdoc />
	public void SetupDashboard(IMenuProvider dashboardProvider)
	{
		dashboardProvider
			.AccessMenu(SienarMenuNames.Dashboard.MyAccount)
			.AddMenuLink(new()
				{
					Text = "Email",
					Icon = Icons.Material.Filled.Email,
					Url = DashboardUrls.Account.EmailChange.Index
				})
			.AddMenuLink(new()
				{
					Text = "Password",
					Icon = Icons.Material.Filled.Lock,
					Url = DashboardUrls.Account.PasswordChange.Index
				})
			.AddMenuLink(new()
				{
					Text = "Personal data",
					Icon = Icons.Material.Filled.Archive,
					Url = DashboardUrls.Account.PersonalData
				});
	}

	public void SetupStyles(IStyleProvider styleProvider) {}

	public void SetupScripts(IScriptProvider scriptProvider) {}

	public void SetupComponents(IComponentProvider componentProvider)
	{
		componentProvider.SidebarHeader = typeof(DrawerHeader);
	}

	private static void CreateMainMenu(IMenuProvider menuProvider)
	{
		var accountLink = new MenuLink
		{
			Text = "My account",
			Icon = Icons.Material.Filled.AccountCircle,
			RequireLoggedIn = true,
			Sublinks =
			[
				new()
				{
					Text = "Email",
					Icon = Icons.Material.Filled.Email,
					Url = DashboardUrls.Account.EmailChange.Index
				},
				new()
				{
					Text = "Password",
					Icon = Icons.Material.Filled.Lock,
					Url = DashboardUrls.Account.PasswordChange.Index
				},
				new()
				{
					Text = "Personal data",
					Icon = Icons.Material.Filled.Archive,
					Url = DashboardUrls.Account.PersonalData
				}
			]
		};

		menuProvider
			.AccessMenu(SienarMenuNames.MainMenu)
			.AddMenuLink(accountLink)
			.AddMenuLink(
				new()
				{
					Text = "Users",
					Icon = Icons.Material.Filled.SupervisorAccount,
					Url = DashboardUrls.Users.Index,
					Roles = [Roles.Admin]
				})
			.AddMenuLink(
				new()
				{
					Text = "Log out",
					Icon = Icons.Material.Filled.Logout,
					Url = DashboardUrls.Account.Logout,
					RequireLoggedIn = true
				})
			.AddMenuLink(
				new()
				{
					Text = "Register",
					Icon = Icons.Material.Filled.Assignment,
					Url = DashboardUrls.Account.Register.Index,
					RequireLoggedOut = true
				})
			.AddMenuLink(
				new()
				{
					Text = "Log in",
					Icon = Icons.Material.Filled.Login,
					Url = DashboardUrls.Account.Login,
					RequireLoggedOut = true
				});
	}

	private static void CreateInfoMenu(IMenuProvider menuProvider)
	{
		menuProvider
			.AccessMenu(SienarMenuNames.InfoMenu)
			.AddMenuLink(
				new()
				{
					Text = "About",
					Icon = Icons.Material.Outlined.Info,
					Url = DashboardUrls.About
				});
	}
}