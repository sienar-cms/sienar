using System;

namespace Sienar.Infrastructure.Plugins;

public class PluginData
{
	public required string Name { get; init; } 
	public required string Author { get; init; }
	public required string AuthorUrl { get; init; }
	public required Version Version { get; init; }
	public required string Description { get; init; }
	public string Homepage { get; set; } = string.Empty;
}