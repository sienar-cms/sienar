using System;
using System.Diagnostics.CodeAnalysis;

namespace Sienar.Exceptions;

/// <summary>
/// The base exception used by Sienar
/// </summary>
/// <remarks>
/// Any exception that extends from <c>SienarException</c> may have its error message exposed to end users. Use caution when forming error messages for the <c>SienarException</c> class!
/// </remarks>
[ExcludeFromCodeCoverage]
public class SienarException : Exception
{
	/// <inheritdoc />
	public SienarException()
	{
	}

	/// <inheritdoc />
	public SienarException(string? message) : base(message)
	{
	}

	/// <inheritdoc />
	public SienarException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}