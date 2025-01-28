using System.Threading.Tasks;
using Sienar.Data;

namespace Sienar.Processors;

// ReSharper disable once TypeParameterCanBeVariant
/// <summary>
/// A processor which accepts no input and returns a <see cref="OperationResult{TResult}"/>
/// </summary>
public interface IResultProcessor<TResult>
{
	/// <summary>
	/// Processes the request and generates the result
	/// </summary>
	/// <returns>the result of the operation</returns>
	Task<OperationResult<TResult?>> Process();
}
