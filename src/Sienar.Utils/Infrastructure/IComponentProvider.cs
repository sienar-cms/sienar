using System;
using System.Collections.Generic;

namespace Sienar.Infrastructure;

/// <summary>
/// A provider to contain references to various components to render in the Sienar UI
/// </summary>
public interface IComponentProvider : IDictionaryProvider<Type, Dictionary<Enum, Type>>;
