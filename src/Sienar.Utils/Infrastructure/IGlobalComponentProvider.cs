using System;

namespace Sienar.Infrastructure;

/// <summary>
/// A provider to contain references to various global components to render in the Sienar UI
/// </summary>
public interface IGlobalComponentProvider
{
	/// <summary>
	/// The default layout component to use when no layout is specified
	/// </summary>
	public Type? DefaultLayout { get; set; }

	/// <summary>
	/// The component to render when no route is matched by the router
	/// </summary>
	public Type? NotFoundView { get; set; }

	/// <summary>
	/// The component to render when the user is fails an authorization check
	/// </summary>
	public Type? UnauthorizedView { get; set; }
}
