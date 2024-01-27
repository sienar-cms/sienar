using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor;
using Sienar.Infrastructure.Menus;

namespace Sienar.Infrastructure.Plugins;

public class SienarBlazorPlugin : ISienarPlugin
{
	/// <inheritdoc />
	public PluginData PluginData { get; } = new()
	{
		Name = "Sienar Blazor",
		Version = Version.Parse("0.1.0"),
		Author = "Christian LeVesque",
		AuthorUrl = "https://levesque.dev",
		Description = "Sienar Blazor provides all of the main services and configuration required to operate the Sienar CMS. Sienar cannot serve a Blazor app without this plugin.",
		Homepage = "https://sienar.siteveyor.com"
	};

	/// <inheritdoc />
	public PluginSettings PluginSettings { get; } = new()
	{
		HasRoutableComponents = true
	};

	/// <inheritdoc />
	public void SetupDependencies(WebApplicationBuilder builder)
	{
		SienarUtils.SetupBaseDirectory();

		var services = builder.Services;
		var config = builder.Configuration;

		services
			.AddSienarUtilities()
			.AddSienarIdentity()
			.AddSienarMedia()
			.ConfigureSienarOptions(config)
			.ConfigureSienarBlazor()
			.ConfigureSienarBlazorAuth();
	}

	/// <inheritdoc />
	public void SetupApp(WebApplication app)
	{
		if (!app.Environment.IsDevelopment())
		{
			app
				.UseExceptionHandler("/Error")
				.UseHsts();
		}

		app
			.UseStaticFiles()
			.UseRouting()
			.UseAuthorization();
		app.MapBlazorHub();
		app.MapFallbackToPage("/_Host");

		var menuProvider = app.Services.GetRequiredService<IMenuProvider>();
		CreateMainMenu(menuProvider);
		CreateInfoMenu(menuProvider);
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