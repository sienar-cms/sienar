using System.Threading.Tasks;

namespace Sienar.Hooks;

/// <summary>
/// Runs after-action hooks for a hookable request
/// </summary>
/// <typeparam name="T">the type of the request or entity</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IAfterActionRunner<T>
{
	/// <summary>
	///  Runs all after-action hooks for a hookable request
	/// </summary>
	/// <param name="input">the request or entity</param>
	/// <param name="action">the action type</param>
	Task Run(
		T input,
		ActionType action);
}
