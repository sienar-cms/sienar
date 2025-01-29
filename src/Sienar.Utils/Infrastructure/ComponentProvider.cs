#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;

namespace Sienar.Infrastructure;

/// <exclude />
public class ComponentProvider
	: DictionaryProvider<Type, Dictionary<Enum, Type>>,
		IComponentProvider
{
	public Type? GetComponent(Type type, Enum id)
	{
		if (!TryGetValue(type, out var components)) return null;
		components.TryGetValue(id, out var component);
		return component;
	}
}
