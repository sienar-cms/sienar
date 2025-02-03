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
	protected string MergeCssClasses(string className)
	{
		if (Attributes is null) return className;

		return Attributes.TryGetValue("class", out var classes)
			? $"{classes} {className}"
			: className;
	}

	/// <summary>
	/// Checks if a cached value is different from its current value, and if so, updates the provided <c>ref shouldRerender</c> value to <c>true</c>
	/// </summary>
	/// <param name="cachedValue">The cached value to compare</param>
	/// <param name="newValue">The new value to compare</param>
	/// <param name="shouldRerender">A <c>ref</c> indicating whether the component should rerender because the values are different</param>
	/// <typeparam name="T">The type of the values to compare</typeparam>
	protected static void UpdateCachedValue<T>(
		ref T cachedValue,
		T newValue,
		ref bool shouldRerender)
	{
		// We have to check if the cached value is null separately
		// because we don't want to use a null conditional with a default value
		if (cachedValue is null)
		{
			// Both are null, so do nothing
			if (newValue is null) return;

			// The new value is different, so overwrite and rerender
			cachedValue = newValue;
			shouldRerender = true;
			return;
		}

		// The new value is different, so overwrite and rerender
		if (!cachedValue.Equals(newValue))
		{
			cachedValue = newValue;
			shouldRerender = true;
		}
	}
}
