#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using Microsoft.AspNetCore.Components;

namespace Sienar.Infrastructure;

/// <exclude />
public class ComponentProvider
	: DictionaryProvider<Type, ComponentDictionary>,
		IComponentProvider
{
	public ComponentDictionary Access<TLayout>()
		where TLayout : LayoutComponentBase
		=> Access(typeof(TLayout));
}
