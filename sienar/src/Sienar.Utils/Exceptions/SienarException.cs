using System;

namespace Sienar.Exceptions;

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