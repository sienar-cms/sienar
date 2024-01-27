using System.Collections.Generic;

namespace Sienar.Infrastructure.Menus;

public class Menu : Dictionary<MenuPriority, List<MenuLink>>, IMenu
{
	/// <inheritdoc />
	public IMenu AddMenuLink(
		MenuLink menuLink,
		MenuPriority priority = MenuPriority.Mid)
	{
		if (!TryGetValue(priority, out var menuItems))
		{
			menuItems = [];
			this[priority] = menuItems;
		}

		menuItems.Add(menuLink);

		return this;
	}
}