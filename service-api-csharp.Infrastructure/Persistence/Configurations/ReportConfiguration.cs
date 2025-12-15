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
        builder.Property(r => r.Id).HasColumnName("Id"); // Postgers is case sensitive only if quoted, but Npgsql usually folds to lowercase if not quoted in strict mode? No, default is quotes.
                                                         // Wait, if I use "reports" (lowercase table), and "id" generic...
                                                         // The error said: "r.CategoryId" does not exist. Hint "r.category_id".
                                                         // So I should use snake_case for everything.

        builder.Property(r => r.Id).HasColumnName("Id"); // Wait, verify if Id is snake case "id" or "Id". CategoryReport uses "id". I should use "id" here too.

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("Title"); // Should be "title"? 
            
        // Let's look at `CategoryReportConfiguration`: .HasColumnName("category_name");
        // So I should use "title", "description", etc.

        builder.Property(r => r.Id).HasColumnName("Id"); // NO, "id". 
        // Existing is header "builder.HasKey(r => r.Id);"
        
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
            .HasColumnName("date_report");

        builder.Property(r => r.EmergencyLevel)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("emergency_level");
            
        // num_delete_report_request IS ALREADY DONE.
        // delete_request_user_ids IS ALREADY DONE.

        builder.Property(r => r.UserId).HasColumnName("user_id");
        builder.Property(r => r.CategoryId).HasColumnName("category_id");
        
        builder.Property(r => r.CreatedAt).HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");
        
        // Base properties? Report inherits from BaseEntity?
        // Check Report.cs first.


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
