using System.Collections.Generic;

namespace Sienar.Infrastructure;

// ReSharper disable once TypeParameterCanBeVariant
public interface IListProvider<T>
{
	List<T> Items { get; }
	IListProvider<T> Add(T item);
}