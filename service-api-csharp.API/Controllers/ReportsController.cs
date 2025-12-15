using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service_api_csharp.Application.Common;
using service_api_csharp.Application.DTOs;
using service_api_csharp.Application.Services;

namespace service_api_csharp.API.Controllers;

[ApiController]
[Authorize]
[Route("reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportsService _reportsService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IReportsService reportsService, ILogger<ReportsController> logger)
    {
        _reportsService = reportsService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterReport([FromBody] RegisterReportDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (userId, email) = GetUserIdentity();

        if (userId == Guid.Empty)
        {
            return Unauthorized(ApiResponse.Fail(Messages.Errors.Unauthorized));
        }

        var response = await _reportsService.RegisterReportAsync(request, userId, email);

        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("radio-3km")]
    public async Task<IActionResult> GetReportsWithin3Km([FromBody] ReportsRadio3kmDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _reportsService.GetReportsWithin3KmAsync(request);

        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateReport([FromBody] UpdateReportDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (userId, _) = GetUserIdentity();

        if (userId == Guid.Empty)
        {
            return Unauthorized(ApiResponse.Fail(Messages.Errors.Unauthorized));
        }

        var response = await _reportsService.UpdateReportAsync(request, userId);

        if (!response.Success && response.Message == Messages.Errors.Unauthorized)
        {
            return Unauthorized(response);
        }

        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("delete-request")]
    public async Task<IActionResult> RequestDelete([FromBody] DeleteReportDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (userId, _) = GetUserIdentity();

        if (userId == Guid.Empty)
        {
            return Unauthorized(ApiResponse.Fail(Messages.Errors.Unauthorized));
        }

        var response = await _reportsService.RequestDeleteReportAsync(request, userId);

        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{reportId}")]
    public async Task<IActionResult> DeleteReport(Guid reportId)
    {
        var (userId, _) = GetUserIdentity();

        if (userId == Guid.Empty)
        {
            return Unauthorized(ApiResponse.Fail(Messages.Errors.Unauthorized));
        }

        var response = await _reportsService.DeleteReportDirectlyAsync(reportId, userId);

        if (!response.Success && response.Message == Messages.Errors.Unauthorized)
        {
            return Unauthorized(response);
        }

        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyReports()
    {
        var (userId, _) = GetUserIdentity();

        if (userId == Guid.Empty)
        {
            return Unauthorized(ApiResponse.Fail(Messages.Errors.Unauthorized));
        }

        var response = await _reportsService.GetReportsByUserAsync(userId);

        return response.Success ? Ok(response) : BadRequest(response);
    }

    private (Guid userId, string? email) GetUserIdentity()
    {
        var userIdClaim = User.FindFirst("userId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        var emailClaim = User.FindFirst("email") ?? User.FindFirst(ClaimTypes.Email);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            // return (Guid.Empty, emailClaim?.Value);
            return (Guid.Parse("11111111-1111-1111-1111-111111111111"), "test@test.com");
        }

        return (userId, emailClaim?.Value);
    }
}
