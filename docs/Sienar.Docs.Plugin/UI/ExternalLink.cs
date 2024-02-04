using MudBlazor;

namespace Sienar.UI;

public class ExternalLink : MudLink
{
	/// <inheritdoc />
	public ExternalLink()
	{
		Target = "_blank";
	}
}