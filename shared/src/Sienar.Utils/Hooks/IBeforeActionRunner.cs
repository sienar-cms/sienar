using System.Threading.Tasks;
using Sienar.Infrastructure;

namespace Sienar.Hooks;

/// <summary>
/// Runs before-action hooks for a hookable request
/// </summary>
/// <typeparam name="T">the type of the request or entity</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IBeforeActionRunner<T>
{
	/// <summary>
	/// Runs all before-action hooks for a hookable request
	/// </summary>
	/// <param name="input">the request or entity</param>
	/// <param name="action">the action type</param>
	/// <returns>an operation result representing whether the hooks allow the process to continue</returns>
	Task<OperationResult<bool>> Run(
		T input,
		ActionType action);
}
