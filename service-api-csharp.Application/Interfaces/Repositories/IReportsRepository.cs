using NetTopologySuite.Geometries;
using service_api_csharp.Domain.Entities;

namespace service_api_csharp.Application.InterfacesRepositories;

public interface IReportsRepository
{
    Task AddReportAsync(Report report, CancellationToken cancellationToken = default);
    Task<CategoryReport?> GetCategoryByIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<Report?> GetReportByIdAsync(Guid reportId, CancellationToken cancellationToken = default);
    Task<ICollection<Report>> GetReportsWithinDistanceAsync(Point origin, double distanceInMeters, CancellationToken cancellationToken = default);
    Task<ICollection<Report>> GetReportsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    void UpdateReport(Report report);
    void RemoveReport(Report report);
}
