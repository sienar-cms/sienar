using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Sienar.Infrastructure.Entities;

namespace Sienar;

public static class SienarBlazorUtilsExtensions
{

#region IServiceCollection

	public static TService? GetAndRemoveService<TService>(this IServiceCollection self)
	{
		var service = self.FirstOrDefault(
			s => s.ServiceType == typeof(TService));
		if (service is not null)
		{
			self.Remove(service);
		}

		return (TService?)service?.ImplementationInstance;
	}

	public static void RemoveService<TService>(this IServiceCollection self)
	{
		var service = self.FirstOrDefault(
			s => s.ServiceType == typeof(TService));
		if (service is not null)
		{
			self.Remove(service);
		}
	}

#endregion

#region Entity

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

#endregion

#region AuthenticationState

	public static bool IsAuthenticated(this AuthenticationState authState)
		=> authState.User.Identity?.IsAuthenticated ?? false;

#endregion

#region Enums

	public static string GetDescription(this Enum self)
	{
		var stringified = self.ToString();
		var a = self
			.GetType()
			.GetField(stringified)
			?.GetCustomAttribute<DescriptionAttribute>();
		return a?.Description ?? stringified;
	}

#endregion
}