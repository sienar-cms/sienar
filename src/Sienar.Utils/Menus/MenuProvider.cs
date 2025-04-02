using System;
using Sienar.Infrastructure;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sienar.Menus;

/// <exclude />
public class MenuProvider : DictionaryProvider<Enum, LinkDictionary<MenuLink>>,  IMenuProvider;