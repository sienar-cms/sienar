using Sienar.Infrastructure;

namespace Sienar.Infrastructure;

/// <summary>
/// The <see cref="IDictionaryProvider{TKey, TValue}"/> used to contain <see cref="MenuLink">menu links</see>
/// </summary>
public interface IMenuProvider : IDictionaryProvider<string, LinkDictionary<MenuLink>>;