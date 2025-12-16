using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using service_api_csharp.Domain.Entities;

namespace service_api_csharp.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id");

        builder.Property(u => u.uuid)
            .HasColumnName("uuid")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("first_name");

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("last_name");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("email");
        
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Password)
            .HasColumnType("text")
            .HasColumnName("password");
        

        builder.Property(u => u.IsVerified)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("is_verified");

        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("role");

        builder.Property(u => u.Status)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("status");

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()")
            .HasColumnName("created_at")
            .HasColumnType("timestamp without time zone");

        builder.Property(u => u.UpdatedAt)
            .HasDefaultValueSql("NOW()")
            .HasColumnName("updated_at")
            .HasColumnType("timestamp without time zone");
        
        builder.Property(u => u.LastLoginAt)
            .HasColumnName("last_login_at")
            .HasColumnType("timestamp without time zone");
        
        builder.Property(u => u.LastFailedLoginAttempt)
            .HasColumnName("last_failed_login_at")
            .HasColumnType("timestamp without time zone");
        
        builder.Property(u => u.FailedLoginAttemps)
            .HasColumnName("failed_login_attempts");
    }
}
