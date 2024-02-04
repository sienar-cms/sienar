using System.Collections.Generic;

namespace Sienar.Infrastructure.Plugins;

public interface IStyleProvider
{
	/// <summary>
	/// Enqueues a CSS stylesheet resource with the provided URL
	/// </summary>
	/// <param name="url">The URL to the resource. May be absolute or root-relative</param>
	/// <param name="crossOrigin">The crossorigin mode to use when requesting the resource</param>
	/// <param name="referrerPolicy">The referrer policy to use when requesting the resource</param>
	/// <param name="integrity">The expected hash of the resource</param>
	IStyleProvider Enqueue(
		string url,
		CrossOriginMode? crossOrigin = null,
		ReferrerPolicy? referrerPolicy = null,
		string? integrity = null);

	/// <summary>
	/// Enqueues a CSS stylesheet resource with the provided stylesheet information
	/// </summary>
	/// <param name="resource">The <see cref="StyleResource"/> describing the stylesheet to enqueue</param>
	/// <returns></returns>
	IStyleProvider Enqueue(StyleResource resource);

	/// <summary>
	/// Retrieves an enumerable of all registered <see cref="ScriptResource"/>s
	/// </summary>
	/// <returns>the <see cref="ScriptResource"/> enumerable</returns>
	IEnumerable<StyleResource> GetStylesheets();
}