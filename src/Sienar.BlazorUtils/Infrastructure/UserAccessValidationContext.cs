namespace Sienar.Infrastructure;

public class UserAccessValidationContext
{
	public bool CanAccess { get; private set; }

	public void Approve() => CanAccess = true;
}