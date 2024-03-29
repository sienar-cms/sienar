﻿namespace Sienar.Infrastructure.Plugins;

public class ScriptResource
{
	/// <summary>
	/// The URL to the script resource
	/// </summary>
	/// <remarks>
	/// The URL provided here should either be absolute (e.g., to a CDN link) or root-relative (e.g., <c>/_content/My.Plugin.Assembly/main.js</c>).
	/// </remarks>
	public required string Src { get; init; }

	/// <summary>
	/// Whether the script resource should be included as a <b>module</b>
	/// </summary>
	public bool IsModule { get; init; }

	/// <summary>
	/// Whether the script resource should be included with <b>async</b>
	/// </summary>
	public bool IsAsync { get; init; }

	/// <summary>
	/// Whether the script resource should be included with <b>defer</b>
	/// </summary>
	public bool ShouldDefer { get; init; }

	/// <summary>
	/// The value to use for the <c>crossorigin</c> attribute
	/// </summary>
	public CrossOriginMode? CrossOriginMode { get; init; }

	/// <summary>
	/// The value to use for the <c>referrerpolicy</c> attribute
	/// </summary>
	public ReferrerPolicy? ReferrerPolicy { get; init; }

	/// <summary>
	/// The expected hash of the resource
	/// </summary>
	public string? Integrity { get; init; }

	/// <summary>
	/// Returns <c>"module"</c> if the script is an ES module, otherwise <c>null</c>
	/// </summary>
	public string? GetScriptType()
	{
		return IsModule
			? "module"
			: null;
	}

	public static implicit operator ScriptResource(string source)
		=> new() { Src = source };
}