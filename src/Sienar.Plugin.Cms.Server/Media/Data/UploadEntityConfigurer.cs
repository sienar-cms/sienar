using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sienar.Media.Data;

public class UploadEntityConfigurer : IEntityTypeConfiguration<Upload>
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<Upload> builder)
	{
		builder
			.Property(u => u.Title)
			.HasMaxLength(100)
			.IsRequired();

		builder
			.Property(u => u.Path)
			.HasMaxLength(300)
			.IsRequired();

		builder
			.Property(u => u.Description)
			.HasMaxLength(300)
			.IsRequired();

		builder
			.Ignore(u => u.IsUnassigned)
			.Ignore(u => u.Contents);
	}
}