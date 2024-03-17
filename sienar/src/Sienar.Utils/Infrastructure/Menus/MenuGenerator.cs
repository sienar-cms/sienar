using System.Threading.Tasks;

namespace Sienar.Infrastructure.Menus;

public class MenuGenerator : AuthorizedLinkAggregator<MenuLink>, IMenuGenerator
{
	public MenuGenerator(
		IUserAccessor userAccessor,
		IMenuProvider menuProvider)
		: base(userAccessor, menuProvider) {}

	/// <inheritdoc />
	protected override async Task PerformAdditionalProcessing(MenuLink link)
	{
		if (link.Sublinks is not null)
		{
			link.Sublinks = await ProcessNavLinks(link.Sublinks);
		}
	}
}