using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service_api_csharp.Domain.Entities;
using service_api_csharp.Domain.ValueObjects;
using System.Linq;

namespace service_api_csharp.Infrastructure.Persistence.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("reports");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(r => r.DateReport)
            .IsRequired();

        builder.Property(r => r.EmergencyLevel)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.NumDeleteReportRequest)
            .HasColumnName("num_delete_report_request")
            .HasDefaultValue(0);

        builder.Property(r => r.DeleteRequestUserIds)
            .HasColumnName("delete_request_user_ids");

        builder.HasOne(r => r.User)
            .WithMany(u => u.Reports)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Category)
            .WithMany(c => c.Reports)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(e => e.UbicationCoordinates)
            .HasColumnType("geometry(Point,4326)")
            .HasColumnName("ubication_coordinates");
    }
}
