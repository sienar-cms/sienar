using System.Collections.Generic;

namespace Sienar.Infrastructure;

// ReSharper disable once TypeParameterCanBeVariant
/// <summary>
/// A wrapper around a dictionary that guarantees that an item with the specified key exists prior to access
/// </summary>
/// <typeparam name="TKey">The type of the dictionary key</typeparam>
/// <typeparam name="TValue">The type of the dictionary value</typeparam>
public interface IDictionaryProvider<TKey, TValue> : IDictionary<TKey, TValue>
	where TKey : notnull
{
	/// <summary>
	/// Returns an item to operate on
	/// </summary>
	/// <param name="name">The name of the  specific item</param>
	/// <returns>the item</returns>
	TValue Access(TKey name);
}