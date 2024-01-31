using System;

namespace Sienar.Infrastructure.Plugins;

public interface IComponentProvider
{
	public Type DefaultLayout { get; set; }
	public Type? SidebarHeader { get; set; }
	public Type? SidebarFooter { get; set; }
}