using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Sienar.Configuration;
using Sienar.Extensions;

namespace Sienar.Ui;

/// <summary>
/// A Bulma-styled HTML &lt;progress&gt; element
/// </summary>
public class ProgressBar : SienarComponentBase
{
	/// <summary>
	/// The color of the progess bar
	/// </summary>
	[Parameter]
	public Color Color { get; set; } = ProgressBarDefaults.Color;

	/// <summary>
	/// The size of the progress bar
	/// </summary>
	[Parameter]
	public Size Size { get; set; } = ProgressBarDefaults.Size;

	/// <summary>
	/// The value of the HTML <c>value</c> attribute
	/// </summary>
	[Parameter]
	public float? Value { get; set; }

	/// <summary>
	/// The value of the HTML <c>max</c> attribute
	/// </summary>
	[Parameter]
	public float Max { get; set; } = ProgressBarDefaults.Max;

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		Attributes ??= new Dictionary<string, object>();
		var progress = 0;

		if (Value.HasValue)
		{
			Attributes["value"] = Value.Value;
			Attributes["max"] = Max;
			progress = (int)Math.Floor(Value.Value / Max * 100);
		}
		else
		{
			Attributes.Remove("value");
			Attributes.Remove("max");
		}

		builder.OpenElement(0, "progress");
		builder.AddMultipleAttributes(1, Attributes);
		builder.AddAttribute(2, "class", CreateCssClasses());
		builder.AddContent(3, $"{progress}%");
		builder.CloseElement();
	}

	private string CreateCssClasses()
	{
		var classes = $"progress is-{Color.GetHtmlValue()} is-{Size.GetHtmlValue()}";
		return MergeCssClasses(classes);
	}
}
