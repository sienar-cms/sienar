using System.Linq;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class SienarServiceCollectionExtensions
{
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
}