using Sienar.Infrastructure;

namespace Sienar.Layouts;

public class SienarLayout : SienarLayoutBase
{
	public SienarLayout()
	{
		MenuNames = [
			SienarMenuNames.MainMenu,
			SienarMenuNames.InfoMenu
		];
	}
}