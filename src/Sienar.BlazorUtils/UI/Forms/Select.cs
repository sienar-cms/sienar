using MudBlazor;

namespace Sienar.UI.Forms;

public class Select<T> : MudSelect<T>
{
	/// <inheritdoc />
	public Select()
	{
		AnchorOrigin = Origin.BottomLeft;
		TransformOrigin = Origin.TopLeft;
	}
}