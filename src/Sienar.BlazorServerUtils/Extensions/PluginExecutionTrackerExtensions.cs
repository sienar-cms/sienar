using Microsoft.AspNetCore.Http;
using Sienar.Infrastructure.Plugins;

namespace Sienar.Extensions;

public static class PluginExecutionTrackerExtensions
{
	public static bool ExecuteAsSubApp(
		this IPluginExecutionTracker self,
		HttpContext httpContext,
		PathString routeSegment)
	{
		if (self.SubAppHasExecuted) return false;
		if (httpContext.Request.Path.StartsWithSegments(routeSegment))
		{
			self.ClaimExecution();
			return true;
		}

		return false;
	}
}