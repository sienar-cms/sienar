using System.Collections.Generic;

namespace Sienar.Infrastructure;

// ReSharper disable once TypeParameterCanBeVariant
/// <summary>
/// A wrapper around a list
/// </summary>
/// <typeparam name="T">the type of the list items</typeparam>
public interface IListProvider<T>
{
	List<T> Items { get; }
	IListProvider<T> Add(T item);
}