using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;
using Sienar.Infrastructure.Services;

namespace Sienar;

public static class SienarBlazorUtilsExtensions
{

#region IServiceCollection

	public static IServiceCollection AddSienarBlazorUtilities(
		this IServiceCollection self)
	{
		self.TryAddSingleton<IMenuProvider, MenuProvider>();
		self.TryAddScoped<IMenuGenerator, MenuGenerator>();
		self.TryAddScoped<IStyleProvider, StyleProvider>();
		self.TryAddScoped<IScriptProvider, ScriptProvider>();
		self.TryAddScoped(typeof(IDbContextAccessor<>), typeof(DbContextAccessor<>));
		self.TryAddTransient<INotificationService, NotificationService>();
		self.TryAddTransient(typeof(IEntityReader<>), typeof(EntityReader<>));
		self.TryAddTransient(typeof(IEntityWriter<>), typeof(EntityWriter<>));
		self.TryAddTransient(typeof(IEntityDeleter<>), typeof(EntityDeleter<>));

		return self;
	}

	public static object? GetAndRemoveService(
		this IServiceCollection self,
		Type serviceType)
	{
		var service = self.FirstOrDefault(
			s => s.ServiceType == serviceType);
		if (service is not null)
		{
			self.Remove(service);
		}

		return service?.ImplementationInstance;
	}

	public static TService? GetAndRemoveService<TService>(
		this IServiceCollection self)
		=> (TService?)GetAndRemoveService(self, typeof(TService));

	public static void RemoveService(
		this IServiceCollection self,
		Type serviceType)
	{
		var service = self.FirstOrDefault(
			s => s.ServiceType == serviceType);
		if (service is not null)
		{
			self.Remove(service);
		}
	}

	public static void RemoveService<TService>(this IServiceCollection self)
		=> RemoveService(self, typeof(TService));

#endregion

#region Middleware

	public static void UsePluginMiddleware<TPlugin>(this IApplicationBuilder self)
		where TPlugin : ISienarPlugin
		=> self.UseMiddleware<SienarPluginMiddleware<TPlugin>>();

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

	/// <summary>
	/// Gets the HTML-expected value of <see cref="ReferrerPolicy"/> and <see cref="CrossOriginMode"/> members
	/// </summary>
	/// <param name="self">the referrer policy or cross-origin mode to get a value for</param>
	/// <returns>the value if the enum is not null and the value is defined, else null</returns>
	public static string? GetHtmlValue(this Enum? self)
	{
		return self?
			.GetType()
			.GetField(self.ToString())?
			.GetCustomAttribute<HtmlValueAttribute>()
			?.Value;
	}

#endregion
}