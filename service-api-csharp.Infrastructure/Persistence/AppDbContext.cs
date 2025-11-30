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
    public DbSet<EmergencySite> EmergencyCities { get; set; }
    public DbSet<CitySector> CitySectors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Ejemplo para PostgreSQL (Npgsql)
        optionsBuilder.UseNpgsql("DefaultConnection",
            x => x.UseNetTopologySuite() // <--- Esto habilita el mapeo espacial de NTS
        );

        // O para SQL Server (si fuera el caso):
        // optionsBuilder.UseSqlServer("TuCadenaDeConexion", x => x.UseNetTopologySuite());
    }
}