using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service_api_csharp.Domain.Entities;
using service_api_csharp.Domain.ValueObjects;

namespace service_api_csharp.Infrastructure.Persistence.Configurations;

public class EmergencySiteConfiguration : IEntityTypeConfiguration<EmergencySite>
{
    public void Configure(EntityTypeBuilder<EmergencySite> builder)
    {
        builder.ToTable("emergency_sites"); 

        builder.HasKey(e => e.Id);

        builder.Property(e => e.NameSite)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name_site");

        builder.Property(e => e.Phone)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("phone");

        builder.Property(e => e.NameSite)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name_site");

        // Spatial Mapping for Point
        builder.Property(e => e.UbicationCoordinates)
            .IsRequired()
            .HasColumnType("geometry(Point, 4326)")
            .HasConversion(
                p => new NetTopologySuite.Geometries.Point(p.X, p.Y) { SRID = p.Srid },
                p => Point.Create(p.X, p.Y, p.SRID)
            )
            .HasColumnName("ubication_coordinates");


        builder.Property(e => e.UbicationDirection)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("description");

        builder.HasOne(e => e.Sector)
            .WithMany(s => s.EmergencyCities)
            .HasForeignKey(e => e.SectorId);

        builder.HasOne(e => e.Category)
            .WithMany(c => c.EmergencySites)
            .HasForeignKey(e => e.CategoryId);
    }
}
