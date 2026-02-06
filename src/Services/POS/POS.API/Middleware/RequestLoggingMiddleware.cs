using System.Diagnostics;

namespace POS.API.Middleware;

/// <summary>
/// Middleware for request logging
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? "N/A";

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var request = context.Request;
            var response = context.Response;

            _logger.LogInformation(
                "[{CorrelationId}] {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                correlationId,
                request.Method,
                request.Path,
                response.StatusCode,
                stopwatch.ElapsedMilliseconds);

            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                _logger.LogWarning(
                    "[{CorrelationId}] Slow request: {Method} {Path} took {ElapsedMs}ms",
                    correlationId,
                    request.Method,
                    request.Path,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
