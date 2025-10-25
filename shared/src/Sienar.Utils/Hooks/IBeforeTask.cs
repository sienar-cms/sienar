using System.Threading.Tasks;

namespace Sienar.Hooks;

/// <summary>
/// Performs arbitrary actions before a class performs a specific action
/// </summary>
/// <typeparam name="TRequest">The type of the request or entity</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IBeforeTask<TRequest>
	where TRequest : class
{
	/// <summary>
	/// Performs an arbitrary action before a class performs a specific action
	/// </summary>
	/// <param name="request">The request or entity, if any</param>
	Task Handle(TRequest? request = null);
}