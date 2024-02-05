namespace Sienar.Infrastructure;

// ReSharper disable once TypeParameterCanBeVariant
public interface IDictionaryProvider<T>
{
	/// <summary>
	/// Returns an <see cref="T">item</see> to operate on
	/// </summary>
	/// <param name="name">The name of the  specific item</param>
	/// <returns>the item</returns>
	T Access(string name);
}