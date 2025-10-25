using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Sienar.Infrastructure;

/// <summary>
/// Stores the types of overridable layout sections in a dictionary 
/// </summary>
public class ComponentDictionary : Dictionary<Enum, Type>
{
	/// <summary>
	/// Adds a component type that overrides the given section key
	/// </summary>
	/// <param name="key">The key of the section</param>
	/// <typeparam name="TComponent">The component type</typeparam>
	/// <returns>The component dictionary</returns>
	public ComponentDictionary AddComponent<TComponent>(Enum key)
		where TComponent : IComponent
		=> AddComponent(key, typeof(TComponent));

	/// <summary>
	/// Adds a component type that overrides the given section key
	/// </summary>
	/// <param name="key">The key of the section</param>
	/// <param name="value">The component type</param>
	/// <returns>The component dictionary</returns>
	public ComponentDictionary AddComponent(Enum key, Type value)
	{
		Add(key, value);
		return this;
	}

	/// <summary>
	/// Adds a component type if no component is alredy registered for the given section key
	/// </summary>
	/// <param name="key">The key of the section</param>
	/// <typeparam name="TComponent">The component type</typeparam>
	/// <returns>The component dictionary</returns>
	public ComponentDictionary TryAddComponent<TComponent>(Enum key)
		where TComponent : IComponent
		=> TryAddComponent(key, typeof(TComponent));

	/// <summary>
	/// Adds a component type if no component is alredy registered for the given section key
	/// </summary>
	/// <param name="key">The key of the section</param>
	/// <param name="value">The component type</param>
	/// <returns>The component dictionary</returns>
	public ComponentDictionary TryAddComponent(Enum key, Type value)
	{
		TryAdd(key, value);
		return this;
	}

	/// <summary>
	/// Gets a component with the given key if it exists
	/// </summary>
	/// <param name="key">The key of the section</param>
	/// <returns>The component type, if it exists</returns>
	public Type? GetComponent(Enum key)
	{
		TryGetValue(key, out var component);
		return component;
	}
}