using MudBlazor;

// ReSharper disable once CheckNamespace
namespace Sienar.UI;

public class Checkbox<T> : MudCheckBox<T>
{
	public Checkbox()
	{
		Color = Color.Primary;
		UnCheckedColor = Color.Primary;
	}
}