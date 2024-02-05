using System.Collections.Generic;

namespace Sienar.Infrastructure;

public class DictionaryProvider<T> : IDictionaryProvider<T>
	where T : new()
{
	protected Dictionary<string, T> Items = new();

	public T Access(string name)
	{
		if (!Items.TryGetValue(name, out var item))
		{
			item = new();
			Items[name] = item;
		}

		return item;
	}
}