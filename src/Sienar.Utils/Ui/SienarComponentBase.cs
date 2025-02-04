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
	public Dictionary<string, object>? UserAttributes { get; set; }

	/// <summary>
	/// Adds a CSS class to the <see cref="UserAttributes"/> dictionary while preserving any existing values
	/// </summary>
	/// <param name="className">The CSS class name to add</param>
	protected string MergeCssClasses(string className)
	{
		if (UserAttributes is null) return className;

		return UserAttributes.TryGetValue("class", out var classes)
			? $"{classes} {className}"
			: className;
	}
}
