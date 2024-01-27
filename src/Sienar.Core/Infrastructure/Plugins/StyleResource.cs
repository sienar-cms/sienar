namespace Sienar.Infrastructure.Plugins;

public class StyleResource
{
	/// <summary>
	/// The URL to the script resource
	/// </summary>
	/// <remarks>
	/// The URL provided here should either be absolute (e.g., to a CDN link) or root-relative (e.g., <c>/_content/My.Plugin.Assembly/main.js</c>).
	/// </remarks>
	public required string Url { get; init; }

	/// <summary>
	/// The value to use for the <c>crossorigin</c> attribute
	/// </summary>
	public CrossOriginMode? Mode { get; init; }

	/// <summary>
	/// The value to u se for the <c>referrerpolicy</c> attribute
	/// </summary>
	public ReferrerPolicy? Referrer { get; init; }

	/// <summary>
	/// The expected hash of the resource
	/// </summary>
	public string? Integrity { get; init; }
}