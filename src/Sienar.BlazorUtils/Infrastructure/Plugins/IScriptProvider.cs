using System.Collections.Generic;

namespace Sienar.Infrastructure.Plugins;

public interface IScriptProvider
{
	/// <summary>
	/// Enqueues a JavaScript script resource with the provided URL 
	/// </summary>
	/// <param name="url">The URL to the resource. May be absolute or root-relative</param>
	/// <param name="isModule">Whether the script tag should render as a module</param>
	/// <param name="isAsync">Whether the script tag should include the <c>async</c> attribute</param>
	/// <param name="shouldDefer">Whether the script tag should include the <c>defer</c> attribute</param>
	/// <param name="crossOrigin">The crossorigin mode to use when requesting the resource</param>
	/// <param name="referrerPolicy">The referrer policy to use when requesting the resource</param>
	/// <param name="integrity">The expected hash of the resource</param>
	IScriptProvider Enqueue(
		string url,
		bool isModule = false,
		bool isAsync = false,
		bool shouldDefer = true,
		CrossOriginMode? crossOrigin = null,
		ReferrerPolicy? referrerPolicy = null,
		string? integrity = null);

	/// <summary>
	/// Enqueues a JavaScript script resource with the provided script information
	/// </summary>
	/// <param name="resource">The <see cref="ScriptResource"/> describing the script to enqueue</param>
	IScriptProvider Enqueue(ScriptResource resource);

	/// <summary>
	/// Retrieves an enumerable of all registered <see cref="ScriptResource"/>s
	/// </summary>
	/// <returns>the <see cref="ScriptResource"/> enumerable</returns>
	IEnumerable<ScriptResource> GetScripts();
}