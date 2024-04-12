namespace Sienar.Infrastructure.Hooks;

/// <summary>
/// Represents the result of a hookable operation
/// </summary>
/// <typeparam name="TResult">the type of the result</typeparam>
public readonly struct HookResult<TResult>
{
	/// <summary>
	/// The status of the operation
	/// </summary>
	public readonly HookStatus Status;

	/// <summary>
	/// The value returned from the operation
	/// </summary>
	public readonly TResult? Result;

	/// <summary>
	/// Creates a new instance of <c>HookResult&lt;TResult&gt;</c>
	/// </summary>
	/// <param name="status">the status of the operation</param>
	/// <param name="result">the value returned from the operation</param>
	public HookResult(HookStatus status, TResult? result = default)
	{
		Status = status;
		Result = result;
	}
}