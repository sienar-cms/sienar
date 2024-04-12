namespace Sienar.Infrastructure.Menus;

/// <summary>
/// The <see cref="IDictionaryProvider{T}"/> used to contain <see cref="DashboardLink">dashboard links</see>
/// </summary>
public interface IDashboardProvider : IDictionaryProvider<LinkDictionary<DashboardLink>>;