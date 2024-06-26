using Sienar.Infrastructure;

namespace Sevita.Tools;

public class SevitaTheme : SienarTheme
{
	/// <inheritdoc />
	public SevitaTheme()
	{
		Palette.PrimaryLighten = "#A3F5D6";
		Palette.Primary = "#56C2B6";
		Palette.PrimaryDarken = "#007F6E";
	}
}