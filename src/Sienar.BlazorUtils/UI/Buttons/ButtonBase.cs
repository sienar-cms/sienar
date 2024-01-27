using MudBlazor;

namespace Sienar.UI.Buttons;

public abstract class ButtonBase : MudButton
{
	protected ButtonBase()
	{
		ButtonType = ButtonType.Button;
		Variant = Variant.Filled;
	}
}