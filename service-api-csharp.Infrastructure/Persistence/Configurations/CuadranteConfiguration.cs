using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service_api_csharp.Domain.Entities;

namespace service_api_csharp.Infrastructure.Persistence.Configurations;

public class CuadranteConfiguration : IEntityTypeConfiguration<Cuadrante>
{
    public void Configure(EntityTypeBuilder<Cuadrante> builder)
    {
        builder.ToTable("cuadrantes");
        
        builder.HasKey(x => x.Id);

        builder.Property(c => c.NumberCuadrante)
            .HasMaxLength(200)
            .HasColumnName("number_cuadrante")
            .IsRequired();

        builder.Property(c => c.Phone)
            .HasMaxLength(200)
            .HasColumnName("phone")
            .IsRequired();

        builder.HasOne(c => c.EmergencySite)
            .WithMany(c => c.Cuadrantes)
            .HasForeignKey(c => c.PoliceCAIId);
    }
}