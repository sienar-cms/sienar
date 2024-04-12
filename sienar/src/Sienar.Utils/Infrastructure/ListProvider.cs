#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;

namespace Sienar.Infrastructure;

/// <exclude />
public class ListProvider<T> : IListProvider<T>
{
	public List<T> Items { get; } = [];

	public IListProvider<T> Add(T item)
	{
		Items.Add(item);
		return this;
	}
}