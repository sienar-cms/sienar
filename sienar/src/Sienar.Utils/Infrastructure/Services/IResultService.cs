using System.Threading.Tasks;

namespace Sienar.Infrastructure.Services;

/// <summary>
/// A service that accepts no input and returns output
/// </summary>
/// <typeparam name="TResult">the type of the output</typeparam>
public interface IResultService<TResult>
{
	/// <summary>
	/// Executes the request
	/// </summary>
	/// <returns>the output of the operation, or <c>null</c></returns>
	Task<TResult?> Execute();
}