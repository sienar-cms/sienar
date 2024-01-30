using MudBlazor;

namespace Sienar.UI.Forms;

public class Radio<T> : MudRadio<T>
{
	/// <inheritdoc />
	public Radio()
	{
		Class = "d-flex";
		Color = Color.Primary;
	}
}