using System.Diagnostics;

namespace Common.Observability.Tracing;

/// <summary>
/// Extensions for Activity (distributed tracing)
/// </summary>
public static class ActivityExtensions
{
    public const string ServiceName = "PosWmsIntegration";

    private static readonly ActivitySource ActivitySource = new(ServiceName, "1.0.0");

    /// <summary>
    /// Start a new activity for an operation
    /// </summary>
    public static Activity? StartActivity(
        string operationName,
        ActivityKind kind = ActivityKind.Internal,
        ActivityContext? parentContext = null,
        IEnumerable<KeyValuePair<string, object?>>? tags = null)
    {
        var activity = ActivitySource.StartActivity(
            operationName,
            kind,
            parentContext ?? default,
            tags);

        return activity;
    }

    /// <summary>
    /// Start a consumer activity for Kafka message processing
    /// </summary>
    public static Activity? StartConsumerActivity(
        string topic,
        int partition,
        long offset,
        string? correlationId = null)
    {
        var activity = ActivitySource.StartActivity(
            $"consume {topic}",
            ActivityKind.Consumer);

        activity?.SetTag("messaging.system", "kafka");
        activity?.SetTag("messaging.destination", topic);
        activity?.SetTag("messaging.kafka.partition", partition);
        activity?.SetTag("messaging.kafka.offset", offset);

        if (!string.IsNullOrEmpty(correlationId))
        {
            activity?.SetTag("correlation_id", correlationId);
        }

        return activity;
    }

    /// <summary>
    /// Start a producer activity for Kafka message publishing
    /// </summary>
    public static Activity? StartProducerActivity(
        string topic,
        string? key = null,
        string? correlationId = null)
    {
        var activity = ActivitySource.StartActivity(
            $"publish {topic}",
            ActivityKind.Producer);

        activity?.SetTag("messaging.system", "kafka");
        activity?.SetTag("messaging.destination", topic);

        if (!string.IsNullOrEmpty(key))
        {
            activity?.SetTag("messaging.kafka.message_key", key);
        }

        if (!string.IsNullOrEmpty(correlationId))
        {
            activity?.SetTag("correlation_id", correlationId);
        }

        return activity;
    }

    /// <summary>
    /// Start a database activity
    /// </summary>
    public static Activity? StartDatabaseActivity(
        string operation,
        string? tableName = null)
    {
        var activity = ActivitySource.StartActivity(
            $"db {operation}",
            ActivityKind.Client);

        activity?.SetTag("db.system", "postgresql");
        activity?.SetTag("db.operation", operation);

        if (!string.IsNullOrEmpty(tableName))
        {
            activity?.SetTag("db.sql.table", tableName);
        }

        return activity;
    }

    /// <summary>
    /// Add an exception to the current activity
    /// </summary>
    public static void RecordException(this Activity? activity, Exception exception)
    {
        if (activity is null) return;

        activity.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
        {
            { "exception.type", exception.GetType().FullName },
            { "exception.message", exception.Message },
            { "exception.stacktrace", exception.StackTrace }
        }));
    }

    /// <summary>
    /// Add a custom event to the activity
    /// </summary>
    public static void AddBusinessEvent(
        this Activity? activity,
        string eventName,
        params KeyValuePair<string, object?>[] attributes)
    {
        if (activity is null) return;

        var tags = new ActivityTagsCollection(attributes);
        activity.AddEvent(new ActivityEvent(eventName, tags: tags));
    }

    /// <summary>
    /// Set correlation and causation IDs
    /// </summary>
    public static void SetCorrelationIds(
        this Activity? activity,
        string? correlationId,
        string? causationId = null)
    {
        if (activity is null) return;

        if (!string.IsNullOrEmpty(correlationId))
        {
            activity.SetTag("correlation_id", correlationId);
        }

        if (!string.IsNullOrEmpty(causationId))
        {
            activity.SetTag("causation_id", causationId);
        }
    }
}
