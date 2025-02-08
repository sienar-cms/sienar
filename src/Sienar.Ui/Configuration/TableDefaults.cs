namespace Sienar.Configuration;

/// <summary>
/// Contains configurable default parameter values for tables
/// </summary>
public static class TableDefaults
{
	/// <summary>
	/// Whether tables should have borders by default
	/// </summary>
	public static bool Bordered { get; set; }

	/// <summary>
	/// Whether tables should have striped rows by default
	/// </summary>
	public static bool Striped { get; set; }

	/// <summary>
	/// Whether tables should have narrow cells by default
	/// </summary>
	/// <returns></returns>
	public static bool Narrow { get; set; }

	/// <summary>
	/// Whether tables should have a hover effect on rows by default
	/// </summary>
	public static bool Hoverable { get; set; }

	/// <summary>
	/// Whether tables should be full-width by default
	/// </summary>
	public static bool FullWidth { get; set; }

	/// <summary>
	/// Whether tables should hide the search bar by default
	/// </summary>
	public static bool HideSearch { get; set; }

	/// <summary>
	/// The default number of rows per table page
	/// </summary>
	public static int RowsPerPage { get; set; } = 5;

	/// <summary>
	/// The default set of page size options
	/// </summary>
	public static int[] PageSizeOptions { get; set; } = [ 5, 10, 25 ];
}
