using System.Collections.Generic;

namespace Sienar.Infrastructure.Menus;

public class MenuLink : DashboardLink
{
	/// <summary>
	/// Child links to display in a submenu, if any
	/// </summary>
	public List<MenuLink>? Sublinks;
}