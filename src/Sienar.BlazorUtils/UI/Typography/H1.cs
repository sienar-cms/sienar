using MudBlazor;

namespace Sienar.UI.Typography;

public class H1 : MudText
{
	public H1()
	{
		Typo = Typo.h3;
		Class = "mb-8";
	}
}