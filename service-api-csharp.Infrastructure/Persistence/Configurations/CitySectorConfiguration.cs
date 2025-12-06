using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service_api_csharp.Domain.Entities;
using service_api_csharp.Domain.ValueObjects;
using System.Linq;

namespace service_api_csharp.Infrastructure.Persistence.Configurations;

public class CitySectorConfiguration : IEntityTypeConfiguration<CitySector>
{
    public void Configure(EntityTypeBuilder<CitySector> builder)
    {
        builder.ToTable("city_sectors");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.NamePlace)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Codigo)
            .HasColumnName("codigo");
        
        builder.Property(e => e.Area)
            .HasColumnType("geography(Polygon,4326)")
            .HasColumnName("ubication_coordinates");
    }
}