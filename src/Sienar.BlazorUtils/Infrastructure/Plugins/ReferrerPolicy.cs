namespace Sienar.Infrastructure.Plugins;

public enum ReferrerPolicy
{
	NoReferrer,
	NoReferrerWhenDowngrade,
	Origin,
	OriginWhenCrossOrigin,
	SameOrigin,
	StrictOrigin,
	StrictOriginWhenCrossOrigin
}