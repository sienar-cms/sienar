using MudBlazor;

namespace Sienar.UI.Typography;

public class H2 : MudText
{
	/// <inheritdoc />
	public H2()
	{
		Typo = Typo.h2;
		Class = "mb-8";
	}
}