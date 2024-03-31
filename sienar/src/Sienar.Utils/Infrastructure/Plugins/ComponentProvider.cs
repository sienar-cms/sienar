using System;

namespace Sienar.Infrastructure.Plugins;

/// <inheritdoc />
public class ComponentProvider : IComponentProvider
{
	/// <inheritdoc />
	public Type? DefaultLayout { get; set; }

	/// <inheritdoc />
	public Type? AppbarLeft { get; set; }

	/// <inheritdoc />
	public Type? AppbarRight { get; set; }

	/// <inheritdoc />
	public Type? SidebarHeader { get; set; }

	/// <inheritdoc />
	public Type? SidebarFooter { get; set; }
}