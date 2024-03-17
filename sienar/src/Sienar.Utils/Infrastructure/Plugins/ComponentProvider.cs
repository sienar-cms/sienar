using System;
using System.Collections.Generic;

namespace Sienar.Infrastructure.Plugins;

public class ComponentProvider : IComponentProvider
{
	/// <inheritdoc />
	public Type DefaultLayout { get; set; } = default!;

	/// <inheritdoc />
	public Type? AppbarLeft { get; set; }

	/// <inheritdoc />
	public Type? AppbarRight { get; set; }

	/// <inheritdoc />
	public Type? SidebarHeader { get; set; }

	/// <inheritdoc />
	public Type? SidebarFooter { get; set; }
}