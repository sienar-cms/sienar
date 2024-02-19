using System.Collections.Generic;
using System.Linq;

namespace Sienar.Infrastructure.Menus;

public class AuthorizedLinkAggregator<TLink> : IAuthorizedLinkAggregator<TLink>
	where TLink : DashboardLink
{
	private readonly IUserAccessor _userAccessor;
	private readonly IDictionaryProvider<LinkDictionary<TLink>> _provider;

	public AuthorizedLinkAggregator(
		IUserAccessor userAccessor,
		IDictionaryProvider<LinkDictionary<TLink>> provider)
	{
		_userAccessor = userAccessor;
		_provider = provider;
	}

	/// <inheritdoc />
	public List<TLink> Create(string name)
	{
		var orderedLinks = new List<TLink>();
		var linkDictionary = _provider.Access(name);

		foreach (var i in linkDictionary.Keys.OrderDescending())
		{
			orderedLinks.AddRange(linkDictionary[i]);
		}

		return ProcessNavLinks(orderedLinks);
	}

	protected List<TLink> ProcessNavLinks(List<TLink> navLinks)
	{
		var includedLinks = new List<TLink>();

		foreach (var link in navLinks)
		{
			if (!UserIsAuthorized(link))
			{
				continue;
			}

			PerformAdditionalProcessing(link);

			includedLinks.Add(link);
		}

		return includedLinks;
	}

	protected virtual void PerformAdditionalProcessing(TLink link) {}

	private bool UserIsAuthorized(TLink menuLink)
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