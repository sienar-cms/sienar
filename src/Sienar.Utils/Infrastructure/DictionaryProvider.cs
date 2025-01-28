#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;

namespace Sienar.Infrastructure;

/// <exclude />
public class DictionaryProvider<TKey, TValue> : Dictionary<TKey, TValue>, IDictionaryProvider<TKey, TValue>
	where TKey : notnull
	where TValue : new()
{
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