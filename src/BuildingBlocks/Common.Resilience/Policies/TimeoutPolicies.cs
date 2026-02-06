using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;

namespace Common.Resilience.Policies;

/// <summary>
/// Timeout policies for operations
/// </summary>
public static class TimeoutPolicies
{
    /// <summary>
    /// Create a timeout policy
    /// </summary>
    public static ResiliencePipeline<T> CreateTimeout<T>(
        TimeSpan timeout,
        ILogger? logger = null)
    {
        return new ResiliencePipelineBuilder<T>()
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = timeout,
                OnTimeout = args =>
                {
                    logger?.LogWarning(
                        "Operation timed out after {Timeout}ms",
                        args.Timeout.TotalMilliseconds);
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    /// <summary>
    /// Create an optimistic timeout policy
    /// </summary>
    public static AsyncTimeoutPolicy CreateOptimisticTimeout(
        TimeSpan timeout,
        ILogger? logger = null)
    {
        return Policy.TimeoutAsync(
            timeout,
            TimeoutStrategy.Optimistic,
            onTimeoutAsync: (context, timespan, task) =>
            {
                logger?.LogWarning(
                    "Optimistic timeout triggered after {Timeout}ms",
                    timespan.TotalMilliseconds);
                return Task.CompletedTask;
            });
    }

    /// <summary>
    /// Create a pessimistic timeout policy (for non-cancellable operations)
    /// </summary>
    public static AsyncTimeoutPolicy CreatePessimisticTimeout(
        TimeSpan timeout,
        ILogger? logger = null)
    {
        return Policy.TimeoutAsync(
            timeout,
            TimeoutStrategy.Pessimistic,
            onTimeoutAsync: (context, timespan, task) =>
            {
                logger?.LogWarning(
                    "Pessimistic timeout triggered after {Timeout}ms",
                    timespan.TotalMilliseconds);
                return Task.CompletedTask;
            });
    }

    /// <summary>
    /// Standard timeouts for different operation types
    /// </summary>
    public static class StandardTimeouts
    {
        public static TimeSpan Fast => TimeSpan.FromSeconds(5);
        public static TimeSpan Normal => TimeSpan.FromSeconds(30);
        public static TimeSpan Long => TimeSpan.FromMinutes(2);
        public static TimeSpan VeryLong => TimeSpan.FromMinutes(5);

        public static TimeSpan DatabaseQuery => TimeSpan.FromSeconds(30);
        public static TimeSpan DatabaseCommand => TimeSpan.FromMinutes(1);
        public static TimeSpan KafkaProduce => TimeSpan.FromSeconds(30);
        public static TimeSpan KafkaConsume => TimeSpan.FromMinutes(5);
        public static TimeSpan HttpCall => TimeSpan.FromSeconds(30);
    }
}
