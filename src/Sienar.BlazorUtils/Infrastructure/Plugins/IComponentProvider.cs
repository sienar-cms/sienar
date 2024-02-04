using System;
using System.Collections.Generic;

namespace Sienar.Infrastructure.Plugins;

public interface IComponentProvider
{
	public Type AppComponent { get; set; }
	public Type DefaultLayout { get; set; }
	public List<Type> TopLevelComponents { get; set; }
	public Type AuthorizingView { get; set; }
	public Type NotAuthorizedView { get; set; }
	public Type? SidebarHeader { get; set; }
	public Type? SidebarFooter { get; set; }
}