#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;

namespace Sienar.Infrastructure;

/// <exclude />
public class ComponentProvider
	: DictionaryProvider<Type, ComponentDictionary>,
		IComponentProvider;
