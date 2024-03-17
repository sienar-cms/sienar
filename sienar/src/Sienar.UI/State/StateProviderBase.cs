using System;

namespace Sienar.State;

public class StateProviderBase
{
	protected bool HasChanges;
	protected bool IsFrozen;

	public event Action? OnChange;

	protected void NotifyStateChanged()
	{
		if (IsFrozen)
		{
			HasChanges = true;
			return;
		}

		OnChange?.Invoke();
	}

	public void Freeze() => IsFrozen = true;

	public void Unfreeze()
	{
		IsFrozen = false;

		if (HasChanges)
		{
			NotifyStateChanged();
			HasChanges = false;
		}
	}
}