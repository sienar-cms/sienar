using Microsoft.EntityFrameworkCore;
using Sienar.Identity.Data;
using Sienar.Media.Data;

namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="ModelBuilder"/> extension methods for the <c>Sienar.Plugin.Cms</c> assembly
/// </summary>
public static class SienarCmsModelBuilderExtensions
{
	/// <summary>
	/// Adds Sienar's entity configurations to the model builder
	/// </summary>
	/// <param name="b">The model builder</param>
	/// <returns>The model builder</returns>
	public static ModelBuilder AddSienarCmsModels(
		this ModelBuilder b)
	{
		return b
			.ApplyConfiguration(
				new SienarUserEntityConfigurer())
			.ApplyConfiguration(
				new LockoutReasonEntityConfigurer())
			.ApplyConfiguration(
				new UploadEntityConfigurer());
	}
}
