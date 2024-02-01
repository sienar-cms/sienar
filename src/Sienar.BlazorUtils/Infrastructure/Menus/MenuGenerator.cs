using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Sienar.Infrastructure.Menus;

public class MenuGenerator : IMenuGenerator
{
	private readonly IUserAccessor _userAccessor;
	private readonly IMenuProvider _menuProvider;

	public MenuGenerator(
		IUserAccessor userAccessor,
		[FromKeyedServices(SienarBlazorUtilsServiceKeys.MenuProvider)] IMenuProvider menuProvider)
	{
		_userAccessor = userAccessor;
		_menuProvider = menuProvider;
	}

	/// <inheritdoc/>
	public List<MenuLink> CreateMenu(string menuName)
	{
		var orderedLinks = new List<MenuLink>();
		if (!_menuProvider.Menus.TryGetValue(menuName, out var menu))
			return orderedLinks;

		foreach (var i in menu.Keys.OrderDescending())
		{
			orderedLinks.AddRange(menu[i]);
		}

		return ProcessNavLinks(orderedLinks);
	}

	private List<MenuLink> ProcessNavLinks(IEnumerable<MenuLink> navLinks)
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

	private bool UserIsAuthorized(MenuLink menuLink)
	{
		if (menuLink.RequireLoggedIn && !_userAccessor.IsSignedIn()) return false;
		if (menuLink.RequireLoggedOut && _userAccessor.IsSignedIn()) return false;
		if (menuLink.Roles is null) return true;

		foreach (var role in menuLink.Roles)
		{
			if (_userAccessor.UserInRole(role))
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