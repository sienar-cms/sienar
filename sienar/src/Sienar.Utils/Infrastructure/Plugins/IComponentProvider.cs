using System;
using System.Collections.Generic;

namespace Sienar.Infrastructure.Plugins;

public interface IComponentProvider
{
	public Type DefaultLayout { get; set; }
	public Type? AppbarLeft { get; set; }
	public Type? AppbarRight { get; set; }
	public Type? SidebarHeader { get; set; }
	public Type? SidebarFooter { get; set; }
}