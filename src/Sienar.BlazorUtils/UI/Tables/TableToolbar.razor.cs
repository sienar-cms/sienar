using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Sienar.UI.Tables;

public partial class TableToolbar
{
	private string _search = string.Empty;

	[Parameter]
	public string Search { get; set; } = string.Empty;

	[Parameter]
	public string Title { get; set; } = default!;

	[Parameter]
	public EventCallback<string> SearchChanged { get; set; }

	[Parameter]
	public bool HideSearch { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <inheritdoc />
	protected override async Task OnParametersSetAsync()
	{
		if (_search != Search)
		{
			_search = Search;
			await SearchChanged.InvokeAsync(_search);
		}
	}
}