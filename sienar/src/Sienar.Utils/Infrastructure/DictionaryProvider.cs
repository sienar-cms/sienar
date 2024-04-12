#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;

namespace Sienar.Infrastructure;

/// <exclude />
public class DictionaryProvider<T> : IDictionaryProvider<T>
	where T : new()
{
	private readonly Dictionary<string, T> _items = new();

	public T Access(string name)
	{
		if (!_items.TryGetValue(name, out var item))
		{
			item = new();
			_items[name] = item;
		}

		return item;
	}
}