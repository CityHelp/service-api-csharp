using Microsoft.EntityFrameworkCore;
using service_api_csharp.Domain.Entities;

namespace service_api_csharp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // DbSets ― Tablas
    public DbSet<User> Users { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<PhotoReport> PhotoReports { get; set; }
    public DbSet<CategoryReport> CategoryReports { get; set; }
    public DbSet<EmergencySite> EmergencySite { get; set; }
    public DbSet<CitySector> CitySectors { get; set; }
    public DbSet<EmergencySiteCategories> CategoriesEmergencySites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

