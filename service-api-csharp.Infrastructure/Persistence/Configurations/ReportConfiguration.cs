using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service_api_csharp.Domain.Entities;
    using System.Linq;
using System.Text.Json;

namespace service_api_csharp.Infrastructure.Persistence.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("reports");

        builder.HasKey(r => r.Id);
        
        // I will do:
        builder.Property(r => r.Id).HasColumnName("id");
        
        builder.Property(r => r.Title)
             .IsRequired()
             .HasMaxLength(200)
             .HasColumnName("title");

        builder.Property(r => r.Description)
            .IsRequired()
            .HasColumnType("text")
            .HasColumnName("description");

        builder.Property(r => r.DateReport)
            .IsRequired()
            .HasColumnName("date_report")
            .HasColumnType("timestamp without time zone");

        builder.Property(r => r.EmergencyLevel)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("emergency_level");

        builder.Property(r => r.UserId).HasColumnName("user_id");
        builder.Property(r => r.CategoryId).HasColumnName("category_id");
        
        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp without time zone");
        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp without time zone");

        builder.Property(r => r.NumDeleteReportRequest)
            .HasColumnName("delete_report_request");

        builder.Property(r => r.DeleteRequestUserIds)
            .HasColumnName("delete_request_user_ids")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null)
            );
        
        builder.Property(e => e.UbicationCoordinates)
            .HasColumnType("geometry(Point,4326)")
            .HasColumnName("ubication_coordinates");

        builder.Property(r => r.UbicationDirection)
            .HasColumnName("ubication_direction")
            .IsRequired();
        
        builder.HasOne(r => r.User)
            .WithMany(u => u.Reports)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Category)
            .WithMany(c => c.Reports)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
            
    }
}
