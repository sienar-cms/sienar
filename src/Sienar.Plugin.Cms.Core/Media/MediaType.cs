using Sienar.Configuration;

namespace Sienar.Media;

public enum MediaType
{
	[MediaDirectory("files")]
	Other,

	[MediaDirectory("images")]
	Image,

	[MediaDirectory("documents")]
	Document,

	[MediaDirectory("spreadsheets")]
	Spreadsheet,

	[MediaDirectory("presentations")]
	Presentation,

	[MediaDirectory("executables")]
	Executable
}