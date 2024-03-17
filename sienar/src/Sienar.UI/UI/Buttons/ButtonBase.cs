using MudBlazor;

// ReSharper disable once CheckNamespace
namespace Sienar.UI;

public abstract class ButtonBase : MudButton
{
	protected ButtonBase()
	{
		ButtonType = ButtonType.Button;
		Variant = Variant.Filled;
	}
}