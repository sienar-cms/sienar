using System;
using Sienar.Infrastructure;

namespace Sienar.Menus;

/// <summary>
/// The <see cref="IDictionaryProvider{TKey,TValue}"/> used to contain <see cref="MenuLink">menu links</see>
/// </summary>
public interface IMenuProvider : IDictionaryProvider<Enum, LinkDictionary<MenuLink>>;