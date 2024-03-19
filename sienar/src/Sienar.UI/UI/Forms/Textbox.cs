using MudBlazor;

// ReSharper disable once CheckNamespace
namespace Sienar.UI;

public class Textbox<T> : MudTextField<T>
{
	/// <inheritdoc />
	public Textbox()
	{
		Variant = Variant.Text;
		Immediate = true;
	}
}