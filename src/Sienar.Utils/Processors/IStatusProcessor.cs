using System.Threading.Tasks;
using Sienar.Data;

namespace Sienar.Processors;

// ReSharper disable once TypeParameterCanBeVariant
/// <summary>
/// A processor which accepts a <c>TRequest</c> as input and returns an <c>OperationResult&lt;bool&gt;</c>
/// </summary>
/// <typeparam name="TRequest">The type of the processor input</typeparam>
public interface IStatusProcessor<TRequest>
{
	/// <summary>
	/// Processes the provided request
	/// </summary>
	/// <param name="request">The request input</param>
	/// <returns>The result of the operation, including whether the operation was successful</returns>
	Task<OperationResult<bool>> Process(TRequest request);
}
