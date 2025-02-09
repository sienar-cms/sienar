using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Sienar.Configuration;
using Sienar.Data;

// ReSharper disable once CheckNamespace
namespace Sienar.Ui;

[CascadingTypeParameter(nameof(TItem))]
public partial class Table<TItem>
{
	private bool _loading;
	private int _page;
	private int _pageSize;
	private ICollection<TItem> _items = [];

	/// <summary>
	/// The current sort direction
	/// </summary>
	public SortDirection SortDirection { get; set; }

	/// <summary>
	/// The current search term
	/// </summary>
	public string? SearchTerm { get; set; }

	/// <summary>
	/// The current sort column
	/// </summary>
	public string? SortColumn { get; set; }

	/// <summary>
	/// The title of the table
	/// </summary>
	[Parameter]
	public required string TableTitle { get; set; }

	/// <summary>
	/// Whether the table should have borders
	/// </summary>
	[Parameter]
	public bool Bordered { get; set; } = TableDefaults.Bordered;

	/// <summary>
	/// Whether the table should have striped rows
	/// </summary>
	[Parameter]
	public bool Striped { get; set; } = TableDefaults.Striped;

	/// <summary>
	/// Whether the table should have narrow cells
	/// </summary>
	[Parameter]
	public bool Narrow { get; set; } = TableDefaults.Narrow;

	/// <summary>
	/// Whether the table should have a hover effect on rows
	/// </summary>
	[Parameter]
	public bool Hoverable { get; set; } = TableDefaults.Hoverable;

	/// <summary>
	/// Whether the table should be full-width
	/// </summary>
	[Parameter]
	public bool FullWidth { get; set; } = TableDefaults.FullWidth;

	/// <summary>
	/// Whether the table should hide the search bar
	/// </summary>
	[Parameter]
	public bool HideSearch { get; set; } = TableDefaults.HideSearch;

	/// <summary>
	/// The number of rows per table page
	/// </summary>
	[Parameter]
	public int RowsPerPage { get; set; } = TableDefaults.RowsPerPage;

	/// <summary>
	/// A callback that enabled Blazor two-way data binding on the <see cref="RowsPerPage"/> parameter
	/// </summary>
	[Parameter]
	public EventCallback<int> RowsPerPageChanged { get; set; }

	/// <summary>
	/// The available page size options
	/// </summary>
	[Parameter]
	public int[] PageSizeOptions { get; set; } = TableDefaults.PageSizeOptions;

	/// <summary>
	/// A function that loads data for the table
	/// </summary>
	[Parameter]
	public Func<Filter?, Task<OperationResult<PagedQuery<TItem>>>>? LoadData { get; set; }

	/// <summary>
	/// The items to render in the table
	/// </summary>
	[Parameter]
	public ICollection<TItem>? Items { get; set; }

	/// <summary>
	/// The template to use for &lt;tbody&gt;
	/// </summary>
	[Parameter]
	public required RenderFragment<TItem> RowTemplate { get; set; }

	/// <summary>
	/// The title content to render
	/// </summary>
	[Parameter]
	public required RenderFragment TitleContent { get; set; }

	/// <summary>
	/// The icon content to render
	/// </summary>
	[Parameter]
	public RenderFragment? IconContent { get; set; }

	/// <summary>
	/// The toolbar content for the table
	/// </summary>
	[Parameter]
	public RenderFragment? ToolbarContent { get; set; }

	/// <summary>
	/// The &lt;thead&gt; content of the table
	/// </summary>
	[Parameter]
	public required RenderFragment HeaderContent { get; set; }

	/// <summary>
	/// The content to display if no items exist
	/// </summary>
	[Parameter]
	public RenderFragment? NoRecordsContent { get; set; }

	[Inject]
	private ILogger<Table<TItem>> Logger { get; set; } = null!;

	/// <summary>
	/// Reloads the table data
	/// </summary>
	public async Task RefreshTable()
	{
		_loading = true;
		StateHasChanged();

		var filter = new Filter
		{
			SortDescending = SortDirection == SortDirection.Descending,
			Page = _page + 1, // MudBlazor is 0-indexed
			PageSize = RowsPerPage = _pageSize
		};

		if (SortDirection != SortDirection.None &&
			!string.IsNullOrWhiteSpace(SortColumn))
		{
			filter.SortName = SortColumn;
		}

		if (!string.IsNullOrWhiteSpace(SearchTerm))
		{
			filter.SearchTerm = SearchTerm;
		}

		var result = await LoadData(filter);
		_loading = false;

		_items = result.Result?.Items ?? [];

		StateHasChanged();
	}

	/// <inheritdoc />
	protected override async Task OnInitializedAsync()
	{
		await RefreshTable();
	}

	private string CreateCssClasses()
	{
		var classes = "table";

		if (Bordered) classes += " is-bordered";
		if (Striped) classes += " is-striped";
		if (Narrow) classes += " is-narrow";
		if (Hoverable) classes += " is-hoverable";
		if (FullWidth) classes += " is-fullwidth";

		return MergeCssClasses(classes);
	}

	private void ResetSearch()
	{
		SearchTerm = null;
		_ = RefreshTable();
	}
}
