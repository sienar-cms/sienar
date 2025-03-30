using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sienar.Identity.Data;

internal class SienarRoleEntityConfigurer : IEntityTypeConfiguration<SienarRole>
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<SienarRole> builder)
	{
		builder
			.HasIndex(r => r.NormalizedName)
			.IsUnique();
		builder
			.Property(r => r.NormalizedName)
			.HasMaxLength(100)
			.IsRequired();
		builder
			.Property(r => r.Name)
			.HasMaxLength(100)
			.IsRequired();
	}
}