using System.Collections.Generic;

namespace Sienar.Infrastructure.Menus;

public class MenuProvider : IMenuProvider
{
	/// <inheritdoc />
	public Dictionary<string, Menu> Menus { get; } = new();

	/// <inheritdoc />
	public IMenu AccessMenu(string menuName)
	{
		if (!Menus.TryGetValue(menuName, out var menu))
		{
			menu = new();
			Menus[menuName] = menu;
		}
		return menu;
	}
}