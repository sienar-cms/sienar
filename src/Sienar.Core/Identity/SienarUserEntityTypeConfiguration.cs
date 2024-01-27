using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sienar.Identity;

public class SienarUserEntityTypeConfiguration : IEntityTypeConfiguration<SienarUser>
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<SienarUser> builder)
	{
		builder
			.HasMany(u => u.Roles)
			.WithMany();

		builder
			.HasMany(u => u.Media)
			.WithOne()
			.HasForeignKey(f => f.UserId)
			.IsRequired(false);
	}
}