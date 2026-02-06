using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers;

/// <summary>
/// Health check endpoints
/// </summary>
[ApiController]
[Route("[controller]")]
public sealed class HealthController : ControllerBase
{
    /// <summary>
    /// Basic health check
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTimeOffset.UtcNow,
            Service = "POS.API",
            Version = typeof(HealthController).Assembly.GetName().Version?.ToString() ?? "1.0.0"
        });
    }

    /// <summary>
    /// Readiness check
    /// </summary>
    [HttpGet("ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult Ready()
    {
        // TODO: Check database connectivity, Kafka connectivity, etc.
        return Ok(new { Status = "Ready" });
    }

    /// <summary>
    /// Liveness check
    /// </summary>
    [HttpGet("live")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Live()
    {
        return Ok(new { Status = "Live" });
    }
}
