using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace service_api_csharp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestAuthController : ControllerBase
{
    private readonly ILogger<TestAuthController> _logger;

    public TestAuthController(ILogger<TestAuthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Endpoint protegido que requiere autenticación JWT
    /// </summary>
    [Authorize]
    [HttpGet("protected")]
    public IActionResult GetProtectedData()
    {
        var userId = User.FindFirst("sub")?.Value;
        var userName = User.FindFirst("name")?.Value ?? User.Identity?.Name;
        var email = User.FindFirst("email")?.Value;

        _logger.LogInformation("Usuario autenticado accediendo a recurso protegido: {UserId}", userId);

        return Ok(new
        {
            message = "¡Acceso autorizado! 🎉",
            user = new
            {
                id = userId,
                name = userName,
                email = email,
                claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            }
        });
    }

    /// <summary>
    /// Endpoint de prueba para verificar el estado del servicio
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "CityHelp API"
        });
    }
}
