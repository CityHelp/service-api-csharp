using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using service_api_csharp.Application.InterfacesRepositories;
using service_api_csharp.Domain.Entities;
using service_api_csharp.Infrastructure.Persistence;

namespace service_api_csharp.Infrastructure.Repositories;

public class ReportsRepository : IReportsRepository
{
    private readonly AppDbContext _context;

    public ReportsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddReportAsync(Report report, CancellationToken cancellationToken = default)
    {
        await _context.Reports.AddAsync(report, cancellationToken);
    }

    public async Task<CategoryReport?> GetCategoryByIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.CategoryReports.FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
    }

    public async Task<Report?> GetReportByIdAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Include(r => r.Photos)
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.Id == reportId, cancellationToken);
    }

    public async Task<ICollection<Report>> GetReportsWithinDistanceAsync(Point origin, double distanceInMeters, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Include(r => r.Photos)
            .Include(r => r.Category)
            .Where(r => EF.Functions.IsWithinDistance(r.UbicationCoordinates, origin, distanceInMeters))
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Report>> GetReportsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Include(r => r.Photos)
            .Include(r => r.Category)
            .Where(r => r.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public void UpdateReport(Report report)
    {
        _context.Reports.Update(report);
    }

    public void RemoveReport(Report report)
    {
        _context.Reports.Remove(report);
    }
}
