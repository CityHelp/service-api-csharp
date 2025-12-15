using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
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
    [Route("register")]
    public async Task<IActionResult> RegisterReport([FromBody] RegisterReportDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserIdentity();

        if (userId is null)
            return Unauthorized();

        var response = await _reportsService.RegisterReportAsync(request, userId.Value);

        if (!response.Success)
        {
            if (response.Message == Messages.Errors.UnexpectedError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            
            return BadRequest(response);
        }
        
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("radio-3km")]
    public async Task<IActionResult> GetReportsWithin3Km([FromBody] ReportsRadio3kmDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _reportsService.GetReportsWithin3KmAsync(request);

        if (!response.Success)
        {
            if (response.Message == Messages.Errors.UnexpectedError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            
            return BadRequest(response);
        }
        
        return Ok(response);
    }

    // [HttpPut]
    // public async Task<IActionResult> UpdateReport([FromBody] UpdateReportDto request)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(ModelState);
    //     }
    //
    //     var userId = GetUserIdentity();
    //
    //     if (userId is null)
    //     {
    //         return Unauthorized();
    //     }
    //
    //     var response = await _reportsService.UpdateReportAsync(request, userId.Value);
    //
    //     if (!response.Success)
    //     {
    //         if (response.Message == Messages.Errors.Unauthorized)
    //         {
    //             return Unauthorized();
    //         }
    //
    //         if (response.Message == Messages.Errors.UnexpectedError)
    //         {
    //             return StatusCode(StatusCodes.Status500InternalServerError, response);
    //         }
    //         return BadRequest(response);
    //     }
    //
    //     return Ok(response);
    // }

    [HttpPut]
    [Route("request-delete")]
     public async Task<IActionResult> RequestDelete([FromBody] DeleteReportDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserIdentity();

        if (userId is null)
        {
            return Unauthorized();
        }

        var response = await _reportsService.RequestDeleteReportAsync(request, userId.Value);
        
        if (!response.Success)
        {
            if (response.Message == Messages.Errors.Unauthorized)
            {
                return Unauthorized();
            }

            if (response.Message == Messages.Errors.UnexpectedError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete]
    [Route("delete/{reportId}")]
    public async Task<IActionResult> DeleteReport(Guid reportId)
    {
        var userId = GetUserIdentity();

        if (userId is null)
        {
            return Unauthorized();
        }

        var response = await _reportsService.DeleteReportDirectlyAsync(reportId, userId.Value);

        if (!response.Success && response.Message == Messages.Errors.Unauthorized)
        {
            return Unauthorized();
        }

        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyReports()
    {
        var userId = GetUserIdentity();

        if (userId is null)
        {
            return Unauthorized();
        }

        var response = await _reportsService.GetReportsByUserAsync(userId.Value);
        
        if (!response.Success)
        {
            if (response.Message == Messages.Errors.Unauthorized)
            {
                return Unauthorized();
            }

            if (response.Message == Messages.Errors.UnexpectedError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return BadRequest(response);
        }

        return Ok(response);
    }

    private int? GetUserIdentity()
    {
        var claim = User.FindFirst("userId")?.Value;

        if (string.IsNullOrWhiteSpace(claim))
            return null;

        if (!int.TryParse(claim, out var userId))
            return null;

        return userId;
    }
}
