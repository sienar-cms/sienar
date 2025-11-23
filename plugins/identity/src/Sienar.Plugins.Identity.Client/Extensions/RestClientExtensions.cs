using System.Threading.Tasks;
using Sienar.Infrastructure;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="IRestClient"/> extension methods used by the <c>Sienar.Plugins.Identity.Client</c> assembly
/// </summary>
public static class RestClientExtensions
{
	/// <summary>
	/// Refreshes the app CSRF token in the browser
	/// </summary>
	/// <param name="self">The rest client</param>
	public static Task RefreshCsrfToken(this IRestClient self)
		=> self.SendRaw("csrf");
}
