using System.Collections.Generic;

namespace Sienar.Infrastructure;

public class ListProvider<T> : IListProvider<T>
{
	/// <inheritdoc />
	public List<T> Items { get; } = [];

	/// <inheritdoc />
	public IListProvider<T> Add(T item)
	{
		Items.Add(item);
		return this;
	}
}