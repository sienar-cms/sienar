#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sienar.Extensions;
using Sienar.Security;

namespace Sienar.Infrastructure;

/// <exclude />
public class MenuGenerator : IMenuGenerator
{
	private readonly IUserAccessor _userAccessor;
	private readonly IMenuProvider _provider;

	public MenuGenerator(
		IUserAccessor userAccessor,
		IMenuProvider menuProvider)
	{
		_userAccessor = userAccessor;
		_provider = menuProvider;
	}

	/// <inheritdoc />
	public Task<List<MenuLink>> Create(Enum name)
	{
		var linkDictionary = _provider.Access(name);
		return ProcessNavLinks(linkDictionary.AggregatePrioritized());
	}

	private async Task<List<MenuLink>> ProcessNavLinks(List<MenuLink> navLinks)
	{
		var includedLinks = new List<MenuLink>();

		foreach (var link in navLinks)
		{
			if (!await UserIsAuthorized(link))
			{
				continue;
			}

			if (link.ChildMenu is not null)
			{
				link.Text ??= link.ChildMenu.GetDescription();
				link.Sublinks = await Create(link.ChildMenu);
			}

			includedLinks.Add(link);
		}

		return includedLinks;
	}

	private async Task<bool> UserIsAuthorized(MenuLink menuLink)
	{
		if (menuLink.RequireLoggedIn && !await _userAccessor.IsSignedIn()) return false;
		if (menuLink.RequireLoggedOut && await _userAccessor.IsSignedIn()) return false;
		if (menuLink.Roles is null) return true;

		foreach (var role in menuLink.Roles)
		{
			if (await _userAccessor.UserInRole(role))
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