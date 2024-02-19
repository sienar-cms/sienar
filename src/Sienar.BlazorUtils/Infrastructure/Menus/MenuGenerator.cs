namespace Sienar.Infrastructure.Menus;

public class MenuGenerator : AuthorizedLinkAggregator<MenuLink>, IMenuGenerator
{
	public MenuGenerator(
		IUserAccessor userAccessor,
		IMenuProvider menuProvider)
		: base(userAccessor, menuProvider) {}

	/// <inheritdoc />
	protected override void PerformAdditionalProcessing(MenuLink link)
	{
		if (link.Sublinks is not null)
		{
			link.Sublinks = ProcessNavLinks(link.Sublinks);
		}
	}
}