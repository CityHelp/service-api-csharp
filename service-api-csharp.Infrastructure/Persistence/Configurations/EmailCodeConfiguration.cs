using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service_api_csharp.Domain.Entities;

namespace service_api_csharp.Infrastructure.Persistence.Configurations;

public class EmailCodeConfiguration : IEntityTypeConfiguration<EmailVerificationCode>
{
    public void Configure(EntityTypeBuilder<EmailVerificationCode> builder)
    {
        // Mapeo de Tabla
        builder.ToTable("email_verification_codes");
            
        // Clave Primaria (Id: serial PK NOT NULL -> int)
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id");
        // .ValueGeneratedOnAdd() // Opcional, si serial es generado por la DB

        // User ID (user_id: int8 -> long)
        builder.Property(e => e.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        // Code (code: varchar(200) NOT NULL)
        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("code");

        // Expires At (expires_at: timestampz -> DateTimeOffset)
        builder.Property(e => e.ExpiresAt)
            .IsRequired()
            .HasColumnName("expires_at")
            .HasColumnType("timestamp without time zone");

        // Created At (created_at: timestampz -> DateTimeOffset)
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at")
            .HasColumnType("timestamp without time zone");

        // Is Used (is_used: bool -> bool)
        builder.Property(e => e.IsUsed)
            .IsRequired()
            .HasColumnName("is_used");
            
        // Attempts (attempts: int4 -> int)
        builder.Property(e => e.Attempts)
            .IsRequired()
            .HasColumnName("attempts");
        
        builder.HasOne(e => e.User)
            .WithMany(u => u.EmailVerificationCodes) // Asumiendo que ApplicationUser tiene una colección
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
