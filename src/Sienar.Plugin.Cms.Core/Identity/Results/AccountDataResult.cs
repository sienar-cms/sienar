namespace Sienar.Identity.Results;

/// <summary>
/// Contains the account data of the currently logged in account
/// </summary>
public class AccountDataResult
{
	/// <summary>
	/// The username of the currently logged in account
	/// </summary>
	public required string Username { get; set; }

	/// <summary>
	/// The roles of the currently logged in account
	/// </summary>
	public required string[] Roles { get; set; }
}
