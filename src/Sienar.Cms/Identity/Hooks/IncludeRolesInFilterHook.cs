using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Hooks;

namespace Sienar.Identity.Hooks;

public class IncludeRolesInFilterHook : IBeforeRead<SienarUser>
{
	/// <inheritdoc />
	public Filter Handle(Filter? filter, bool isSingle)
		=> filter ?? new Filter { Includes = ["Roles"] };
}