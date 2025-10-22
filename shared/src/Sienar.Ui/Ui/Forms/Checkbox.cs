using MudBlazor;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

public class Checkbox<T> : MudCheckBox<T>
{
	public Checkbox()
	{
		Color = Color.Primary;
		UncheckedColor = Color.Primary;
	}
}