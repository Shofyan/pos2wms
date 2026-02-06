namespace Common.Events.Abstractions;

/// <summary>
/// Interface for handling events
/// </summary>
public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    /// <summary>
    /// Handle the event
    /// </summary>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of event handling
/// </summary>
public record EventHandlingResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public bool ShouldRetry { get; init; }
    public TimeSpan? RetryAfter { get; init; }

    public static EventHandlingResult Ok() => new() { Success = true };

    public static EventHandlingResult Fail(string message, bool shouldRetry = true, TimeSpan? retryAfter = null)
        => new()
        {
            Success = false,
            ErrorMessage = message,
            ShouldRetry = shouldRetry,
            RetryAfter = retryAfter
        };
}
