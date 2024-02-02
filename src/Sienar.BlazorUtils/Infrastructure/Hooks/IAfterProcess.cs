using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

// ReSharper disable once TypeParameterCanBeVariant
public interface IAfterProcess<TRequest>
{
	Task Handle(TRequest request, ActionType action);
}