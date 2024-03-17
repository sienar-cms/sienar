namespace Sienar.Infrastructure;

public class AccessValidationContext
{
	public bool CanAccess { get; private set; }

	public void Approve() => CanAccess = true;
}