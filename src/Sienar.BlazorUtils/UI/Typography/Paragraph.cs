using MudBlazor;

namespace Sienar.UI.Typography;

public class Paragraph : MudText
{
	public Paragraph()
	{
		Typo = Typo.body1;
		Class = "mb-4";
	}
}