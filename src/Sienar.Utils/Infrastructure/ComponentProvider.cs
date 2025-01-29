using System;
using System.Collections.Generic;

namespace Sienar.Infrastructure;

/// <exclude />
public class ComponentProvider
	: DictionaryProvider<Type, Dictionary<Enum, Type>>,
		IComponentProvider;
