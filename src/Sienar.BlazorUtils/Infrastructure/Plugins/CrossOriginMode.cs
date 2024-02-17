namespace Sienar.Infrastructure.Plugins;

public enum CrossOriginMode
{
	[HtmlValue("")]
	None,

	[HtmlValue("anonymous")]
	Anonymous,

	[HtmlValue("use-credentials")]
	UseCredentials
}