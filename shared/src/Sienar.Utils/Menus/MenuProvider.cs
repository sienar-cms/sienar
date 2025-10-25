using System;
using Sienar.Infrastructure;

namespace Sienar.Menus;

/// <summary>
/// The <see cref="DictionaryProvider{TKey,TValue}"/> used to contain <see cref="MenuLink">menu links</see>
/// </summary>
public class MenuProvider : DictionaryProvider<Enum, LinkDictionary<MenuLink>>;