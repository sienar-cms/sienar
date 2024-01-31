using System;

namespace Sienar.Infrastructure.Plugins;

public class ComponentProvider : IComponentProvider
{
	/// <inheritdoc />
	public Type? SidebarHeader { get; set; }

	/// <inheritdoc />
	public Type? SidebarFooter { get; set; }
}