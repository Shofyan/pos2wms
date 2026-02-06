using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace Common.Resilience.Policies;

/// <summary>
/// Circuit breaker policies for fault tolerance
/// </summary>
public static class CircuitBreakerPolicies
{
    /// <summary>
    /// Create a circuit breaker policy
    /// </summary>
    public static ResiliencePipeline<T> CreateCircuitBreaker<T>(
        int failureThreshold = 5,
        TimeSpan breakDuration = default,
        ILogger? logger = null)
    {
        if (breakDuration == default)
            breakDuration = TimeSpan.FromSeconds(30);

        return new ResiliencePipelineBuilder<T>()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<T>
            {
                FailureRatio = 0.5,
                MinimumThroughput = failureThreshold,
                SamplingDuration = TimeSpan.FromSeconds(30),
                BreakDuration = breakDuration,
                OnOpened = args =>
                {
                    logger?.LogWarning(
                        "Circuit breaker opened for {Duration}. Last exception: {Exception}",
                        breakDuration,
                        args.Outcome.Exception?.Message ?? "None");
                    return ValueTask.CompletedTask;
                },
                OnClosed = args =>
                {
                    logger?.LogInformation("Circuit breaker closed, resuming normal operation");
                    return ValueTask.CompletedTask;
                },
                OnHalfOpened = args =>
                {
                    logger?.LogInformation("Circuit breaker half-opened, testing connection");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    /// <summary>
    /// Create a circuit breaker for database operations
    /// </summary>
    public static AsyncCircuitBreakerPolicy CreateDatabaseCircuitBreaker(
        int failureThreshold = 5,
        TimeSpan? breakDuration = null,
        ILogger? logger = null)
    {
        var duration = breakDuration ?? TimeSpan.FromSeconds(30);

        return Policy
            .Handle<Exception>(ex => IsDatabaseException(ex))
            .CircuitBreakerAsync(
                failureThreshold,
                duration,
                onBreak: (exception, timespan) =>
                {
                    logger?.LogWarning(
                        exception,
                        "Database circuit breaker opened for {Duration}s",
                        timespan.TotalSeconds);
                },
                onReset: () =>
                {
                    logger?.LogInformation("Database circuit breaker reset");
                },
                onHalfOpen: () =>
                {
                    logger?.LogInformation("Database circuit breaker half-open");
                });
    }

    /// <summary>
    /// Create a circuit breaker for Kafka operations
    /// </summary>
    public static AsyncCircuitBreakerPolicy CreateKafkaCircuitBreaker(
        int failureThreshold = 10,
        TimeSpan? breakDuration = null,
        ILogger? logger = null)
    {
        var duration = breakDuration ?? TimeSpan.FromMinutes(1);

        return Policy
            .Handle<Exception>(ex => IsKafkaException(ex))
            .CircuitBreakerAsync(
                failureThreshold,
                duration,
                onBreak: (exception, timespan) =>
                {
                    logger?.LogWarning(
                        exception,
                        "Kafka circuit breaker opened for {Duration}s",
                        timespan.TotalSeconds);
                },
                onReset: () =>
                {
                    logger?.LogInformation("Kafka circuit breaker reset");
                },
                onHalfOpen: () =>
                {
                    logger?.LogInformation("Kafka circuit breaker half-open");
                });
    }

    /// <summary>
    /// Create an advanced circuit breaker with percentage-based failure detection
    /// </summary>
    public static AsyncCircuitBreakerPolicy CreateAdvancedCircuitBreaker(
        double failureThresholdPercentage = 0.5,
        int minimumThroughput = 10,
        TimeSpan? samplingDuration = null,
        TimeSpan? breakDuration = null,
        ILogger? logger = null)
    {
        var sampling = samplingDuration ?? TimeSpan.FromSeconds(30);
        var breaking = breakDuration ?? TimeSpan.FromSeconds(30);

        return Policy
            .Handle<Exception>()
            .AdvancedCircuitBreakerAsync(
                failureThresholdPercentage,
                sampling,
                minimumThroughput,
                breaking,
                onBreak: (exception, circuitState, timespan, context) =>
                {
                    logger?.LogWarning(
                        exception,
                        "Advanced circuit breaker opened (state: {State}) for {Duration}s",
                        circuitState, timespan.TotalSeconds);
                },
                onReset: context =>
                {
                    logger?.LogInformation("Advanced circuit breaker reset");
                },
                onHalfOpen: () =>
                {
                    logger?.LogInformation("Advanced circuit breaker half-open");
                });
    }

    private static bool IsDatabaseException(Exception exception)
    {
        return exception.GetType().Name.Contains("Npgsql") ||
               exception.GetType().Name.Contains("Postgres") ||
               exception.GetType().Name.Contains("Database");
    }

    private static bool IsKafkaException(Exception exception)
    {
        return exception.GetType().Name.Contains("Kafka") ||
               exception.GetType().Name.Contains("Confluent");
    }
}
