namespace Common.Events.Abstractions;

/// <summary>
/// Base interface for all integration events
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Unique identifier for this event instance
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// When the event occurred
    /// </summary>
    DateTimeOffset OccurredAt { get; }

    /// <summary>
    /// Event type name for routing
    /// </summary>
    string EventType { get; }

    /// <summary>
    /// Correlation ID for distributed tracing
    /// </summary>
    string? CorrelationId { get; }

    /// <summary>
    /// Causation ID linking to parent event
    /// </summary>
    string? CausationId { get; }
}

/// <summary>
/// Base implementation for integration events
/// </summary>
public abstract record IntegrationEvent : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public abstract string EventType { get; }
    public string? CorrelationId { get; init; }
    public string? CausationId { get; init; }

    /// <summary>
    /// Version of the event schema
    /// </summary>
    public int SchemaVersion { get; init; } = 1;
}
