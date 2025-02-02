using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Sienar.Ui;

/// <summary>
/// A base component with functionality common across most Sienar components
/// </summary>
public class SienarComponentBase : ComponentBase
{
	/// <summary>
	/// Captures all attributes passed to a component and forwards them to the component
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? Attributes { get; set; }

	/// <summary>
	/// Adds a CSS class to the <see cref="Attributes"/> dictionary while preserving any existing values
	/// </summary>
	/// <param name="className">The CSS class name to add</param>
	protected void AddCssClass(string className)
	{
		if (Attributes is null) return;

		if (Attributes.TryGetValue("class", out var classes))
		{
			Attributes["class"] = $"{classes} {className}";
		}
		else
		{
			Attributes["class"] = className;
		}
	}
}
