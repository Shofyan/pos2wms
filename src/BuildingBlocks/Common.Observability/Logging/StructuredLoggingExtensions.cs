using Microsoft.Extensions.Logging;

namespace Common.Observability.Logging;

/// <summary>
/// Extensions for structured logging
/// </summary>
public static class StructuredLoggingExtensions
{
    /// <summary>
    /// Log with structured event data
    /// </summary>
    public static void LogEvent(
        this ILogger logger,
        LogLevel level,
        string eventName,
        string message,
        params (string Key, object? Value)[] properties)
    {
        using (logger.BeginScope(CreateScope(eventName, properties)))
        {
            logger.Log(level, message);
        }
    }

    /// <summary>
    /// Log a business event
    /// </summary>
    public static void LogBusinessEvent(
        this ILogger logger,
        string eventType,
        Guid entityId,
        string message,
        params (string Key, object? Value)[] additionalProperties)
    {
        var props = new List<(string Key, object? Value)>
        {
            ("EventType", eventType),
            ("EntityId", entityId.ToString()),
            ("EventCategory", "Business")
        };
        props.AddRange(additionalProperties);

        using (logger.BeginScope(CreateScope("BusinessEvent", props.ToArray())))
        {
            logger.LogInformation(message);
        }
    }

    /// <summary>
    /// Log an integration event
    /// </summary>
    public static void LogIntegrationEvent(
        this ILogger logger,
        string eventType,
        Guid eventId,
        string correlationId,
        string message,
        params (string Key, object? Value)[] additionalProperties)
    {
        var props = new List<(string Key, object? Value)>
        {
            ("EventType", eventType),
            ("EventId", eventId.ToString()),
            ("CorrelationId", correlationId),
            ("EventCategory", "Integration")
        };
        props.AddRange(additionalProperties);

        using (logger.BeginScope(CreateScope("IntegrationEvent", props.ToArray())))
        {
            logger.LogInformation(message);
        }
    }

    /// <summary>
    /// Log a performance metric
    /// </summary>
    public static void LogPerformance(
        this ILogger logger,
        string operationName,
        TimeSpan duration,
        bool success,
        params (string Key, object? Value)[] additionalProperties)
    {
        var props = new List<(string Key, object? Value)>
        {
            ("OperationName", operationName),
            ("DurationMs", duration.TotalMilliseconds),
            ("Success", success),
            ("EventCategory", "Performance")
        };
        props.AddRange(additionalProperties);

        using (logger.BeginScope(CreateScope("PerformanceMetric", props.ToArray())))
        {
            if (success)
            {
                logger.LogInformation(
                    "Operation {OperationName} completed in {DurationMs}ms",
                    operationName, duration.TotalMilliseconds);
            }
            else
            {
                logger.LogWarning(
                    "Operation {OperationName} failed after {DurationMs}ms",
                    operationName, duration.TotalMilliseconds);
            }
        }
    }

    private static Dictionary<string, object?> CreateScope(
        string eventName,
        (string Key, object? Value)[] properties)
    {
        var scope = new Dictionary<string, object?>
        {
            ["EventName"] = eventName,
            ["Timestamp"] = DateTimeOffset.UtcNow
        };

        foreach (var (key, value) in properties)
        {
            scope[key] = value;
        }

        return scope;
    }
}
