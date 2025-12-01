using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service_api_csharp.Domain.Entities;

namespace service_api_csharp.Infrastructure.Persistence.Configurations;

public class CategoryReportConfiguration : IEntityTypeConfiguration<CategoryReport>
{
    public void Configure(EntityTypeBuilder<CategoryReport> builder)
    {
        builder.ToTable("category_reports");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("id")
            .UseIdentityAlwaysColumn(); // GENERATED ALWAYS AS IDENTITY

        builder.Property(c => c.CategoryName)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("category_name");

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("description");
    }
}
