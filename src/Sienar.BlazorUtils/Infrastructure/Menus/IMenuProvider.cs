using System.Collections.Generic;

namespace Sienar.Infrastructure.Menus;

public interface IMenuProvider
{
	/// <summary>
	/// Gets the registered <see cref="MenuLink"/> in the app, organized into named menus and grouped by priority
	/// </summary>
	Dictionary<string, Menu> Menus { get; }

	/// <summary>
	/// Internally sets the name of the menu to operate on
	/// </summary>
	/// <param name="menuName">The name of the menu to operate on</param>
	IMenu AccessMenu(string menuName);
}