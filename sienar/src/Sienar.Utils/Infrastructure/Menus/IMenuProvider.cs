namespace Sienar.Infrastructure.Menus;

/// <summary>
/// The <see cref="IDictionaryProvider{T}"/> used to contain <see cref="MenuLink">menu links</see>
/// </summary>
public interface IMenuProvider : IDictionaryProvider<LinkDictionary<MenuLink>>;