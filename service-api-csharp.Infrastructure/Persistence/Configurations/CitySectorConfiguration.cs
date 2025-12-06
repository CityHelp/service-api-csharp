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

        // Spatial Mapping for Polygon
        builder.Property(c => c.Area)
            .IsRequired()
            .HasColumnType("geometry(Polygon, 4326)")
            .HasConversion(
                poly => new NetTopologySuite.Geometries.Polygon(
                    new NetTopologySuite.Geometries.LinearRing(
                        poly.Coordinates.Select(c => new NetTopologySuite.Geometries.Coordinate(c.X, c.Y)).ToArray()
                    )
                ) { SRID = poly.Srid },
                poly => Polygon.Create(
                    poly.ExteriorRing.Coordinates.Select(c => Point.Create(c.X, c.Y, (int)poly.SRID)), 
                    (int)poly.SRID
                )
            );
    }
}