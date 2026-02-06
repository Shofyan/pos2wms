using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace POS.Application.Behaviors;

/// <summary>
/// Pipeline behavior for request logging
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();

        _logger.LogInformation(
            "[{CorrelationId}] Handling {RequestName}",
            correlationId, requestName);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            stopwatch.Stop();

            _logger.LogInformation(
                "[{CorrelationId}] Handled {RequestName} in {ElapsedMs}ms",
                correlationId, requestName, stopwatch.ElapsedMilliseconds);

            if (stopwatch.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning(
                    "[{CorrelationId}] Long running request: {RequestName} ({ElapsedMs}ms)",
                    correlationId, requestName, stopwatch.ElapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "[{CorrelationId}] Error handling {RequestName} after {ElapsedMs}ms",
                correlationId, requestName, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
