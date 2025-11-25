namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="string"/> extension methods for the <c>Sienar.Utils</c> assembly
/// </summary>
public static class SienarUtilsStringExtensions
{
	/// <summary>
	/// Converts a string to all caps. For use in searching and filtering
	/// </summary>
	/// <param name="input">The string to normalize</param>
	/// <returns>The normalized string</returns>
	public static string ToNormalized(this string input)
		=> input.ToUpperInvariant();
}
