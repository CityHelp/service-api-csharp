using service_api_csharp.Application.Common;
using service_api_csharp.Application.DTOs;

namespace service_api_csharp.Application.Services;

public interface IReportsService
{
    Task<ApiResponse> RegisterReportAsync(RegisterReportDto request, Guid userId, string? userEmail, CancellationToken cancellationToken = default);
    Task<ApiResponse> GetReportsWithin3KmAsync(ReportsRadio3kmDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse> UpdateReportAsync(UpdateReportDto request, Guid userId, CancellationToken cancellationToken = default);
    Task<ApiResponse> RequestDeleteReportAsync(DeleteReportDto request, Guid userId, CancellationToken cancellationToken = default);
    Task<ApiResponse> DeleteReportDirectlyAsync(Guid reportId, Guid userId, CancellationToken cancellationToken = default);
    Task<ApiResponse> GetReportsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
