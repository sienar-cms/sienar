using Sienar.Infrastructure.Entities;

namespace Sienar.Infrastructure.Hooks;

// ReSharper disable once UnusedTypeParameter
public interface IBeforeRead<TEntity>
{
	Filter? Handle(Filter? filter, bool isSingle);
}