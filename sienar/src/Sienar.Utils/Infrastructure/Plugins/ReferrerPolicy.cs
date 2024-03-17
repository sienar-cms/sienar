namespace Sienar.Infrastructure.Plugins;

public enum ReferrerPolicy
{
	[HtmlValue("no-referrer")]
	NoReferrer,

	[HtmlValue("no-referrer-when-downgrade")]
	NoReferrerWhenDowngrade,

	[HtmlValue("origin")]
	Origin,

	[HtmlValue("origin-when-cross-origin")]
	OriginWhenCrossOrigin,

	[HtmlValue("same-origin")]
	SameOrigin,

	[HtmlValue("strict-origin")]
	StrictOrigin,

	[HtmlValue("strict-origin-when-cross-origin")]
	StrictOriginWhenCrossOrigin,

	[HtmlValue("unsafe-url")]
	UnsafeUrl
}