using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using Sienar.Infrastructure;

namespace Sienar.UI;

public class Prism : ComponentBase
{
	private ElementReference _codeBlock;
	private string _codeClass = default!;
	private string? _codeContent;

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public string? CodeFile { get; set; }

	[Parameter]
	public Languages Language { get; set; } = Languages.None;

	[Parameter]
	public string Class { get; set; } = "my-8";

	[Parameter]
	public bool Block { get; set; }

	[Inject]
	private IJSRuntime Js { get; set; } = default!;

	[Inject]
	private HttpClient Client { get; set; } = default!;

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		// <pre>
		if (Block)
		{
			builder.OpenElement(0, "pre");
			builder.AddAttribute(1, "class", Class);
		}

		// <code>
		builder.OpenElement(2, "code");
		builder.AddAttribute(3, "class", _codeClass);

		// Set ref
		builder.AddElementReferenceCapture(4, el => _codeBlock = el);

		// Add child content
		if (!string.IsNullOrEmpty(_codeContent))
		{
			builder.AddContent(5, _codeContent);
		}
		else
		{
			builder.AddContent(6, ChildContent);
		}

		// </code>
		builder.CloseElement();

		// </pre>
		if (Block) builder.CloseElement();
	}

	/// <inheritdoc />
	protected override async Task OnInitializedAsync()
	{
		_codeClass = CreateCssClasses();
		if (!string.IsNullOrEmpty(CodeFile))
		{
			_codeContent = await Client.GetStringAsync(CodeFile);
		}
	}

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await Js.InvokeVoidAsync("Prism.highlightElement", _codeBlock);
	}

	private string CreateCssClasses()
	{
		return $"language-{Language.ToString().ToLowerInvariant()}";
	}
}