namespace Sienar.Infrastructure;

/// <summary>
/// Configures <c>I____Provider</c> instances per request
/// </summary>
public interface IRequestConfigurer
{
	/// <summary>
	/// Configures any Sienar providers which should be configured on a per-request basis
	/// </summary>
	void Configure();
}