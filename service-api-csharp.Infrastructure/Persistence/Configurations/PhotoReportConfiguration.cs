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
        
        builder.Property(p => p.Id).HasColumnName("id");

        builder.Property(p => p.PhotoUrl)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("photo_url");

        builder.HasOne(p => p.Report)
            .WithOne(r => r.Photo)
            .HasForeignKey<PhotoReport>(p => p.ReportId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Property(p => p.ReportId).HasColumnName("report_id");
    }
}
