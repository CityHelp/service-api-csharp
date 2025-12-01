using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service_api_csharp.Domain.Entities;
using service_api_csharp.Domain.ValueObjects;

namespace service_api_csharp.Infrastructure.Persistence.Configurations;

public class EmergencyCityConfiguration : IEntityTypeConfiguration<EmergencyCity>
{
    public void Configure(EntityTypeBuilder<EmergencyCity> builder)
    {
        builder.ToTable("emergency_cites"); // Note: ERD says 'emergency_cites', keeping it as is.

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Phone)
            .IsRequired()
            .HasMaxLength(200);

        // Spatial Mapping for Point
        builder.Property(e => e.UbicationCoordinates)
            .IsRequired()
            .HasColumnType("geometry(Point, 4326)")
            .HasConversion(
                p => new NetTopologySuite.Geometries.Point(p.X, p.Y) { SRID = p.Srid },
                p => Point.Create(p.X, p.Y, p.SRID)
            );

        builder.Property(e => e.UbicationDirection)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(e => e.Sector)
            .WithMany(s => s.EmergencyCities)
            .HasForeignKey(e => e.IdSector)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
