using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sienar.Menus;

/// <summary>
/// Generates a list of <see cref="MenuLink">menu links</see> the user is authorized to see
/// </summary>
public interface IMenuGenerator
{
	/// <summary>
	/// Creates a list of authorized <c>TLink</c> instances to be rendered
	/// </summary>
	/// <param name="name">The name of the menu to create</param>
	/// <returns></returns>
	Task<List<MenuLink>> Create(Enum name);
}