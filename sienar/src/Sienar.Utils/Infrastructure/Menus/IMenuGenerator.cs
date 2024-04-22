namespace Sienar.Infrastructure.Menus;
/// <summary>
/// The <see cref="IAuthorizedLinkAggregator{TLink}"/> used to generate a list of <see cref="MenuLink">menu links</see> the user is authorized to see
/// </summary>
public interface IMenuGenerator : IAuthorizedLinkAggregator<MenuLink>;