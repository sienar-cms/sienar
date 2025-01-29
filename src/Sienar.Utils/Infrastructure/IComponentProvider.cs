using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Sienar.Infrastructure;

/// <summary>
/// A provider to contain references to various components to render in the Sienar UI
/// </summary>
public interface IComponentProvider : IDictionaryProvider<Type, ComponentDictionary>
{
	/// <summary>
	/// Accesses the specified layout
	/// </summary>
	/// <typeparam name="TLayout">The type of the layout</typeparam>
	/// <returns>The component dictionary</returns>
	ComponentDictionary Access<TLayout>()
		where TLayout : LayoutComponentBase;
}