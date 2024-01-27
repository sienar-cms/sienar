using Sienar.State;

namespace Sienar.Identity;

public class AccountStateProvider : StateProviderBase
{
	private SienarUser? _user;

	public SienarUser? User
	{
		get => _user;
		set
		{
			if (_user == value)
			{
				return;
			}

			_user = value;
			NotifyStateChanged();
		}
	}
}