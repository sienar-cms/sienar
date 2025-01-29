#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;

namespace Sienar.Infrastructure;

/// <exclude />
public class GlobalComponentProvider : IGlobalComponentProvider
{
	public Type? DefaultLayout { get; set; }
	public Type? NotFoundView { get; set; }
	public Type? UnauthorizedView { get; set; }
}
