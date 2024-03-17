namespace Sienar.Infrastructure.Hooks;

public readonly struct HookResult<TResult>
{
	public readonly HookStatus Status;
	public readonly TResult? Result;

	public HookResult(HookStatus status, TResult? result = default)
	{
		Status = status;
		Result = result;
	}
}