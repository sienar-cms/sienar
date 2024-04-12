using System.Threading.Tasks;

namespace Sienar.Infrastructure.Services;

// ReSharper disable once TypeParameterCanBeVariant
/// <summary>
/// A service that accepts input and returns output
/// </summary>
/// <typeparam name="TRequest">the type of the input</typeparam>
/// <typeparam name="TResult">the type of the output</typeparam>
public interface IService<TRequest, TResult>
{
	/// <summary>
	/// Executes the request
	/// </summary>
	/// <param name="request">the input of the operation</param>
	/// <returns>the output of the operation, or <c>null</c></returns>
	Task<TResult?> Execute(TRequest request);
}