namespace Sienar.Infrastructure.Hooks;

public enum HookStatus
{
	Success,
	NotFound,
	Unauthorized,
	Unprocessable,
	Conflict,
	Concurrency,
	Unknown
}