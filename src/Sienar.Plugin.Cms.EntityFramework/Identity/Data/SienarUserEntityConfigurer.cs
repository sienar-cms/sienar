using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sienar.Identity.Data;

internal class SienarUserEntityConfigurer : IEntityTypeConfiguration<SienarUser>
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<SienarUser> builder)
	{
		builder.Property(u => u.Username)
			.HasMaxLength(50)
			.IsRequired();
		builder
			.HasIndex(u => u.NormalizedUsername)
			.IsUnique();
		builder
			.Property(u => u.NormalizedUsername)
			.HasMaxLength(50)
			.IsRequired();

		builder
			.Property(u => u.Email)
			.HasMaxLength(100)
			.IsRequired();
		builder
			.HasIndex(u => u.NormalizedEmail)
			.IsUnique();
		builder
			.Property(u => u.NormalizedEmail)
			.HasMaxLength(100)
			.IsRequired();

		builder
			.Property(u => u.NormalizedPendingEmail)
			.HasMaxLength(100);
		builder
			.Property(u => u.PendingEmail)
			.HasMaxLength(100);

		builder
			.Property(u => u.PasswordHash)
			.HasMaxLength(100);

		builder
			.Ignore(u => u.Password)
			.Ignore(u => u.ConfirmPassword);
	}
}