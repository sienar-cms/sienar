using System;
using System.Collections.Generic;

namespace Sienar.Infrastructure;

/// <summary>
/// A provider to contain references to various components to render in the Sienar UI
/// </summary>
public interface IComponentProvider : IDictionaryProvider<Type, Dictionary<Enum, Type>>
{
	/// <summary>
	/// Gets a component from the component provider using the provided layout type and component ID
	/// </summary>
	/// <param name="type">The type of the layout that renders the component</param>
	/// <param name="id">The ID of the layout section defined by the component</param>
	/// <returns>The component type, if one is found</returns>
	Type? GetComponent(Type type, Enum id);
}