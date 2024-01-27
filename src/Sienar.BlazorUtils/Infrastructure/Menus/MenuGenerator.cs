using System.Collections.Generic;
using System.Linq;

namespace Sienar.Infrastructure.Menus;

public class MenuGenerator : IMenuGenerator
{
	protected readonly IUserAccessor UserAccessor;
	protected readonly IMenuProvider MenuProvider;

	public MenuGenerator(IUserAccessor userAccessor, IMenuProvider menuProvider)
	{
		UserAccessor = userAccessor;
		MenuProvider = menuProvider;
	}

	/// <inheritdoc/>
	public List<MenuLink> CreateMenu(string menuName)
	{
		var orderedLinks = new List<MenuLink>();
		foreach (var i in MenuProvider.Menus[menuName].Keys.OrderDescending())
		{
			orderedLinks.AddRange(MenuProvider.Menus[menuName][i]);
		}
		return ProcessNavLinks(orderedLinks);
	}

	protected List<MenuLink> ProcessNavLinks(IEnumerable<MenuLink> navLinks)
	{
		var includedLinks = new List<MenuLink>();

		foreach (var link in navLinks)
		{
			if (!UserIsAuthorized(link))
			{
				continue;
			}

			if (link.Sublinks is not null)
			{
				link.Sublinks = ProcessNavLinks(link.Sublinks);
			}

			includedLinks.Add(link);
		}

		return includedLinks;
	}

	protected bool UserIsAuthorized(MenuLink menuLink)
	{
		if (menuLink.RequireLoggedIn && !UserAccessor.IsSignedIn()) return false;
		if (menuLink.RequireLoggedOut && UserAccessor.IsSignedIn()) return false;
		if (menuLink.Roles is null) return true;

		foreach (var role in menuLink.Roles)
		{
			if (UserAccessor.UserInRole(role))
			{
				if (menuLink.AllRolesRequired)
				{
					continue;
				}

				return true;
			}

			if (menuLink.AllRolesRequired)
			{
				return false;
			}
		}

		return menuLink.AllRolesRequired;
	}
}