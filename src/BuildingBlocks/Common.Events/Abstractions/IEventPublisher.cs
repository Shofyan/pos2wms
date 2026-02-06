namespace Common.Events.Abstractions;

/// <summary>
/// Interface for publishing events to message broker
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publish an event to the specified topic
    /// </summary>
    Task PublishAsync<TEvent>(TEvent @event, string topic, CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    /// <summary>
    /// Publish an event with a specific partition key
    /// </summary>
    Task PublishAsync<TEvent>(TEvent @event, string topic, string partitionKey, CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    /// <summary>
    /// Publish multiple events in a batch
    /// </summary>
    Task PublishBatchAsync<TEvent>(IEnumerable<TEvent> events, string topic, CancellationToken cancellationToken = default)
        where TEvent : IEvent;
}
