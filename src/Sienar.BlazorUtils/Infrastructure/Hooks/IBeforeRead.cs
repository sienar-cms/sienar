using Sienar.Infrastructure.Entities;

namespace Sienar.Infrastructure.Hooks;

public interface IBeforeRead<SienarUser>
{
	Filter? Handle(Filter? filter, bool isSingle);
}