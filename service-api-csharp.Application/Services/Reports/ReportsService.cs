using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using service_api_csharp.Application.Common;
using service_api_csharp.Application.DTOs;
using service_api_csharp.Application.InterfacesRepositories;
using service_api_csharp.Domain.Entities;

namespace service_api_csharp.Application.Services;

public class ReportsService : IReportsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly GeometryFactory _geometryFactory;
    private readonly ILogger<ReportsService> _logger;

    public ReportsService(IUnitOfWork unitOfWork, GeometryFactory geometryFactory, ILogger<ReportsService> logger)
    {
        _unitOfWork = unitOfWork;
        _geometryFactory = geometryFactory;
        _logger = logger;
    }

    public async Task<ApiResponse> RegisterReportAsync(RegisterReportDto request, int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!int.TryParse(request.IdCategory, out var categoryId))
            {
                return ApiResponse.Fail(Messages.Errors.GenericField, Messages.ReportsCategories.CategoryNotFound);
            }

            if (!double.TryParse(request.Latitude, out var latitude) || !double.TryParse(request.Longitude, out var longitude))
            {
                return ApiResponse.Fail(Messages.Coordinates.GeneralInvalidCoordinates);
            }

            var category = await _unitOfWork.Reports.GetCategoryByIdAsync(categoryId, cancellationToken);

            if (category is null)
            {
                return ApiResponse.Fail(Messages.Errors.GenericField, Messages.ReportsCategories.CategoryNotFound);
            }

            var ubication = _geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
            ubication.SRID = 4326;

            var report = new Report
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                EmergencyLevel = request.EmergencyLevel.ToUpper(),
                UserId = userId,
                CategoryId = categoryId,
                DateReport = request.DateReport,
                UbicationCoordinates = ubication,
                UbicationDirection = request.DirectionReport,
                NumDeleteReportRequest = 0,
                DeleteRequestUserIds = null,
                CreatedAt = DateTime.UtcNow,
                Photo = !string.IsNullOrEmpty(request.ImageUrl) ? new PhotoReport
                {
                    PhotoUrl = request.ImageUrl
                } : null
            };

            await _unitOfWork.BeginTransactionAsync();

            await _unitOfWork.Reports.AddReportAsync(report, cancellationToken);    
            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ApiResponse.Ok(Messages.Reports.ReportRegistered);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error registering report for user {UserId}", userId);
            return ApiResponse.Fail(Messages.Errors.UnexpectedError);
        }
    }

    public async Task<ApiResponse> GetReportsWithin3KmAsync(ReportsRadio3kmDto request, CancellationToken cancellationToken = default)
    {
        if (!double.TryParse(request.Latitude, out var latitude) || !double.TryParse(request.Longitude, out var longitude))
        {
            return ApiResponse.Fail(Messages.Coordinates.GeneralInvalidCoordinates);
        }

        try
        {
            var origin = _geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
            origin.SRID = 4326;
            var reports = await _unitOfWork.Reports.GetReportsWithinDistanceAsync(origin, 3000, cancellationToken);

            var response = reports.Select(report => new
            {
                report.Id,
                report.Title,
                report.Description,
                report.EmergencyLevel,
                report.CreatedAt,
                Latitude = report.UbicationCoordinates.Y,
                Longitude = report.UbicationCoordinates.X,
                Category = report.Category.CategoryName,
                PhotoUrl = report.Photo.PhotoUrl
            });

            return ApiResponse.Ok(Messages.Reports.ReportsFound, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reports within 3km");
            return ApiResponse.Fail(Messages.Errors.UnexpectedError);
        }
    }

    public async Task<ApiResponse> UpdateReportAsync(UpdateReportDto request, int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var report = await _unitOfWork.Reports.GetReportByIdAsync(request.ReportId, cancellationToken);

            if (report is null)
            {
                return ApiResponse.Fail(Messages.Errors.GenericField, Messages.Reports.ReportNotFound);
            }

            if (report.UserId != userId)
            {
                return ApiResponse.Fail(Messages.Errors.Unauthorized);
            }

            report.Title = request.Title;
            report.Description = request.Description;
            report.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Reports.UpdateReport(report);
            await _unitOfWork.SaveAsync();

            return ApiResponse.Ok(Messages.Reports.ReportUpdated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating report {ReportId}", request.ReportId);
            return ApiResponse.Fail(Messages.Errors.UnexpectedError);
        }
    }

    public async Task<ApiResponse> RequestDeleteReportAsync(DeleteReportDto request, int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var report = await _unitOfWork.Reports.GetReportByIdAsync(request.ReportId, cancellationToken);

            if (report is null)
            {
                return ApiResponse.Fail(Messages.Errors.GenericField, Messages.Reports.ReportNotFound);
            }

            if (report.DeleteRequestUserIds.Contains(userId))
            {
                return ApiResponse.Fail(Messages.Errors.GenericField, Messages.Reports.AlreadyRequestedDeletion);
            }

            report.DeleteRequestUserIds.Add(userId);
            report.NumDeleteReportRequest++;

            if (report.NumDeleteReportRequest >= 3)
            {
                _unitOfWork.Reports.RemoveReport(report);
            }
            else
            {
                _unitOfWork.Reports.UpdateReport(report);
            }

            await _unitOfWork.SaveAsync();

            return report.NumDeleteReportRequest >= 3
                ? ApiResponse.Ok(Messages.Reports.ReportDeleted)
                : ApiResponse.Ok(Messages.Reports.RegisteredDeleteRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing delete request for report {ReportId}", request.ReportId);
            return ApiResponse.Fail(Messages.Errors.UnexpectedError);
        }
    }

    public async Task<ApiResponse> DeleteReportDirectlyAsync(Guid reportId, int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var report = await _unitOfWork.Reports.GetReportByIdAsync(reportId, cancellationToken);

            if (report is null)
            {
                return ApiResponse.Fail(Messages.Errors.GenericField, Messages.Reports.ReportNotFound);
            }

            if (report.UserId != userId)
            {
                return ApiResponse.Fail(Messages.Errors.Unauthorized);
            }

            _unitOfWork.Reports.RemoveReport(report);
            await _unitOfWork.SaveAsync();

            return ApiResponse.Ok(Messages.Reports.ReportDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting report {ReportId}", reportId);
            return ApiResponse.Fail(Messages.Errors.UnexpectedError);
        }
    }
    

    public async Task<ApiResponse> GetReportsByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var reports = await _unitOfWork.Reports.GetReportsByUserAsync(userId, cancellationToken);
            // Timezone logic removed as per requirement for timestamp without time zone
            var response = reports.Select(report => new RegisterReportDto
            {
                Title = report.Title,
                Description = report.Description,
                Category = report.Category.CategoryName,
                IdCategory = report.CategoryId.ToString(),
                EmergencyLevel = report.EmergencyLevel,
                DateReport = report.DateReport,
                Latitude = report.UbicationCoordinates.Y.ToString(),
                Longitude = report.UbicationCoordinates.X.ToString(),
                DirectionReport = report.UbicationDirection,
                ImageUrl = report.Photo?.PhotoUrl
            }).ToList();

            return ApiResponse.Ok(null, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reports for user {UserId}", userId);
            return ApiResponse.Fail(Messages.Errors.UnexpectedError, ex.Message + " " + ex.InnerException?.Message);
        }
    }
    public async Task<ApiResponse> GetReportByIdAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        try
        {
            var report = await _unitOfWork.Reports.GetReportByIdAsync(reportId, cancellationToken);
            
            if (report is null)
            {
                return ApiResponse.Fail(Messages.Errors.GenericField, Messages.Reports.ReportNotFound);
            }

            var response = new RegisterReportDto
            {
                Title = report.Title,
                Description = report.Description,
                Category = report.Category.CategoryName,
                IdCategory = report.CategoryId.ToString(),
                EmergencyLevel = report.EmergencyLevel,
                DateReport = report.DateReport,
                Latitude = report.UbicationCoordinates.Y.ToString(),
                Longitude = report.UbicationCoordinates.X.ToString(),
                DirectionReport = report.UbicationDirection,
                ImageUrl = report.Photo?.PhotoUrl
            };

            return ApiResponse.Ok(null, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving report {ReportId}", reportId);
            return ApiResponse.Fail(Messages.Errors.UnexpectedError);
        }
    }
}
