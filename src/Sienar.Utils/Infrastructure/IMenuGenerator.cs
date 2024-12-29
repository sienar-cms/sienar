using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sienar.Infrastructure;

/// <summary>
/// Generates a list of <see cref="MenuLink">menu links</see> the user is authorized to see
/// </summary>
public interface IMenuGenerator
{
	/// <summary>
	/// Creates a list of authorized <c>TLink</c> instances to be rendered
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	Task<List<MenuLink>> Create(string name);
}