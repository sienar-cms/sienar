using Microsoft.AspNetCore.Mvc;
using Sienar.Infrastructure;

namespace Sienar.Configuration;

/// <summary>
/// Configures ASP.NET MVC to include an <see cref="AutoValidateAntiforgeryTokenAttribute"/>
/// </summary>
public class DefaultMvcConfigurer : IConfigurer<MvcOptions>
{
	/// <inheritdoc />
	public void Configure(MvcOptions options)
	{
		options.Filters.Add(
			new AutoValidateAntiforgeryTokenAttribute());
	}
}