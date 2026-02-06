namespace POS.Domain.Common;

/// <summary>
/// Base interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// When the event occurred
    /// </summary>
    DateTimeOffset OccurredAt { get; }

    /// <summary>
    /// Event type identifier
    /// </summary>
    string EventType { get; }
}

/// <summary>
/// Base record for domain events
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public abstract string EventType { get; }
}
