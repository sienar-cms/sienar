using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MudBlazor;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Menus;
using Sienar.Infrastructure.Plugins;
using Sienar.UI;

namespace Sienar;

public static class SienarBlazorUtilsExtensions
{

#region IServiceCollection

	public static IServiceCollection AddSienarBlazorUtilities(
		this IServiceCollection self)
	{
		self.AddKeyedScoped<IMenuProvider, MenuProvider>(
			SienarBlazorUtilsServiceKeys.MenuProvider);
		self.AddKeyedScoped<IMenuProvider, MenuProvider>(
			SienarBlazorUtilsServiceKeys.DashboardProvider);
		self.AddKeyedScoped<IMenuGenerator, MenuGenerator>(
			SienarBlazorUtilsServiceKeys.MenuProvider);
		self.AddKeyedScoped<IMenuGenerator, DashboardMenuGenerator>(
			SienarBlazorUtilsServiceKeys.DashboardProvider);
		self.TryAddScoped<IRoutableAssemblyProvider, RoutableAssemblyProvider>();
		self.TryAddTransient<INotificationService, NotificationService>();

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

#region IDialogService

	public static async Task<bool> ShowConfirmationDialog(
		this IDialogService self,
		string title,
		string question,
		string confirmText = "yes",
		string cancelText = "no",
		Color mainColor = Color.Primary,
		Color cancelColor = Color.Secondary)
	{
		var parameters = new DialogParameters<ConfirmationDialog>
		{
			{ d => d.Title, title },
			{ d => d.Question, question },
			{ d => d.ConfirmText, confirmText },
			{ d => d.CancelText, cancelText },
			{ d => d.MainColor, mainColor },
			{ d => d.CancelColor, cancelColor }
		};

		var dialog = await self.ShowAsync<ConfirmationDialog>(string.Empty, parameters);

		var result = await dialog.Result;
		return !result.Canceled;
	}

#endregion
}