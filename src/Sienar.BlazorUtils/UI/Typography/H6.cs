using MudBlazor;

namespace Sienar.UI.Typography;

public class H6 : MudText
{
	/// <inheritdoc />
	public H6()
	{
		Typo = Typo.h6;
		Class = "mb-8";
	}
}