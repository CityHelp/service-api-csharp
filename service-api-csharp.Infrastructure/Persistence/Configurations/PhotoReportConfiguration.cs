using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service_api_csharp.Domain.Entities;

namespace service_api_csharp.Infrastructure.Persistence.Configurations;

public class PhotoReportConfiguration : IEntityTypeConfiguration<PhotoReport>
{
    public void Configure(EntityTypeBuilder<PhotoReport> builder)
    {
        builder.ToTable("photos_reports");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PhotoUrl)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(p => p.Report)
            .WithMany(r => r.Photos)
            .HasForeignKey(p => p.ReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
