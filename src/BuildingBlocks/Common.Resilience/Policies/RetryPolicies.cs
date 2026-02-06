using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Common.Resilience.Policies;

/// <summary>
/// Retry policies for various operations
/// </summary>
public static class RetryPolicies
{
    /// <summary>
    /// Create a retry policy with exponential backoff
    /// </summary>
    public static ResiliencePipeline<T> CreateExponentialBackoff<T>(
        int maxRetries = 3,
        TimeSpan? initialDelay = null,
        ILogger? logger = null)
    {
        var delay = initialDelay ?? TimeSpan.FromMilliseconds(100);

        return new ResiliencePipelineBuilder<T>()
            .AddRetry(new RetryStrategyOptions<T>
            {
                MaxRetryAttempts = maxRetries,
                BackoffType = DelayBackoffType.Exponential,
                Delay = delay,
                OnRetry = args =>
                {
                    logger?.LogWarning(
                        args.Outcome.Exception,
                        "Retry attempt {Attempt} after {Delay}ms. Exception: {Exception}",
                        args.AttemptNumber,
                        args.RetryDelay.TotalMilliseconds,
                        args.Outcome.Exception?.Message ?? "None");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    /// <summary>
    /// Create a retry policy for database operations
    /// </summary>
    public static AsyncRetryPolicy CreateDatabaseRetryPolicy(
        int maxRetries = 3,
        ILogger? logger = null)
    {
        return Policy
            .Handle<Exception>(ex => IsDatabaseTransientException(ex))
            .WaitAndRetryAsync(
                maxRetries,
                retryAttempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    logger?.LogWarning(
                        exception,
                        "Database retry {RetryCount}/{MaxRetries} after {Delay}ms",
                        retryCount, maxRetries, timeSpan.TotalMilliseconds);
                });
    }

    /// <summary>
    /// Create a retry policy for Kafka operations
    /// </summary>
    public static AsyncRetryPolicy CreateKafkaRetryPolicy(
        int maxRetries = 5,
        ILogger? logger = null)
    {
        return Policy
            .Handle<Exception>(ex => IsKafkaTransientException(ex))
            .WaitAndRetryAsync(
                maxRetries,
                retryAttempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    logger?.LogWarning(
                        exception,
                        "Kafka retry {RetryCount}/{MaxRetries} after {Delay}ms",
                        retryCount, maxRetries, timeSpan.TotalMilliseconds);
                });
    }

    /// <summary>
    /// Create a forever retry policy for critical operations
    /// </summary>
    public static AsyncRetryPolicy CreateForeverRetryPolicy(
        TimeSpan maxDelay,
        ILogger? logger = null)
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(
                retryAttempt =>
                {
                    var delay = TimeSpan.FromMilliseconds(100 * Math.Pow(2, Math.Min(retryAttempt, 10)));
                    return delay > maxDelay ? maxDelay : delay;
                },
                (exception, retryCount, timeSpan) =>
                {
                    logger?.LogWarning(
                        exception,
                        "Forever retry {RetryCount} after {Delay}ms",
                        retryCount, timeSpan.TotalMilliseconds);
                });
    }

    private static bool IsDatabaseTransientException(Exception exception)
    {
        // PostgreSQL transient error detection
        var message = exception.Message.ToLowerInvariant();
        return message.Contains("connection") ||
               message.Contains("timeout") ||
               message.Contains("deadlock") ||
               message.Contains("too many connections") ||
               message.Contains("temporarily unavailable");
    }

    private static bool IsKafkaTransientException(Exception exception)
    {
        var message = exception.Message.ToLowerInvariant();
        return message.Contains("broker") ||
               message.Contains("timeout") ||
               message.Contains("network") ||
               message.Contains("connection") ||
               message.Contains("leader not available") ||
               message.Contains("not leader");
    }
}
