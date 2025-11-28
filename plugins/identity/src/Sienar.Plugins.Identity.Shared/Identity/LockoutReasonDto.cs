using Sienar.Data;

namespace Sienar.Identity;

/// <summary>
/// A reason why a user might be locked out
/// </summary>
[EntityName(Singular = "lockout reason", Plural = "lockout reasons")]
[RestEndpoint("lockout-reasons")]
public class LockoutReasonDto : EntityBase
{
	/// <summary>
	/// The reason why a user might be locked out
	/// </summary>
	public string Reason { get; set; } = string.Empty;
}
