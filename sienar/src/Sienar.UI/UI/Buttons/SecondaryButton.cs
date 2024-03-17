using MudBlazor;

// ReSharper disable once CheckNamespace
namespace Sienar.UI;

public class SecondaryButton : ButtonBase
{
	public SecondaryButton()
	{
		Color = Color.Secondary;
		Variant = Variant.Outlined;
	}
}