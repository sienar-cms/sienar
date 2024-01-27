using System;
using System.Reflection;

namespace Sienar.Infrastructure.Entities;

public static class EntityExtensions
{
	public static string GetEntityName<TEntity>()
		=> GetEntityName(typeof(TEntity));

	public static string GetEntityName(this EntityBase self)
		=> GetEntityName(self.GetType());

	public static string GetEntityName(this Type self)
	{
		var attribute = self.GetCustomAttribute<EntityNameAttribute>();
		return attribute?.Singular ?? self.Name;
	}

	public static string GetEntityPluralName<TEntity>()
		=> GetEntityPluralName(typeof(TEntity));

	public static string GetEntityPluralName(this EntityBase self)
		=> GetEntityPluralName(self.GetType());

	public static string GetEntityPluralName(this Type self)
	{
		var attribute = self.GetCustomAttribute<EntityNameAttribute>();
		return attribute?.Plural
			?? throw new InvalidOperationException(
				$"Unable to determine plural entity name {self.Name}. "
				+ $"Please ensure you set the entity name with {nameof(EntityNameAttribute)}.");
	}
}