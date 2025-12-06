using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service_api_csharp.Domain.Entities;

namespace service_api_csharp.Infrastructure.Persistence.Configurations;

public class EmergencySiteCategoriesConfiguration : IEntityTypeConfiguration<EmergencySiteCategories>
{
    public void Configure(EntityTypeBuilder<EmergencySiteCategories> builder)
    {
        builder.ToTable("emergency_site_categories");
        
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id");

        builder.Property(c => c.Category)
            .IsRequired()
            .HasColumnName("category")
            .HasMaxLength(200);
    }
}