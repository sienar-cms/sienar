using System;
using System.Collections.Generic;

namespace Sienar.Infrastructure.Plugins;

public class ComponentProvider : IComponentProvider
{
	/// <inheritdoc />
	public Type AppComponent { get; set; } = default!;

	/// <inheritdoc />
	public Type DefaultLayout { get; set; } = default!;

	/// <inheritdoc />
	public List<Type> TopLevelComponents { get; set; } = [];

	/// <inheritdoc />
	public Type AuthorizingView { get; set; } = default!;

	/// <inheritdoc />
	public Type NotAuthorizedView { get; set; } = default!;

	/// <inheritdoc />
	public Type? SidebarHeader { get; set; }

	/// <inheritdoc />
	public Type? SidebarFooter { get; set; }
}