#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;

namespace Sienar.Infrastructure;

// ReSharper disable once TypeParameterCanBeVariant
/// <summary>
/// A wrapper around a dictionary that guarantees that an item with the specified key exists prior to access
/// </summary>
/// <typeparam name="TKey">The type of the dictionary key</typeparam>
/// <typeparam name="TValue">The type of the dictionary value</typeparam>
public class DictionaryProvider<TKey, TValue> : Dictionary<TKey, TValue>
	where TKey : notnull
	where TValue : new()
{
	/// <summary>
	/// Returns an item to operate on
	/// </summary>
	/// <param name="name">The name of the  specific item</param>
	/// <returns>the item</returns>
	public TValue Access(TKey name)
	{
		if (!TryGetValue(name, out var item))
		{
			item = new TValue();
			this[name] = item;
		}

		return item;
	}
}