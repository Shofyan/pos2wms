namespace Common.Kafka.Abstractions;

/// <summary>
/// Interface for Kafka producer
/// </summary>
public interface IKafkaProducer : IAsyncDisposable
{
    /// <summary>
    /// Produce a message to a topic
    /// </summary>
    Task<ProduceResult> ProduceAsync<TValue>(
        string topic,
        string? key,
        TValue value,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Produce a message to a specific partition
    /// </summary>
    Task<ProduceResult> ProduceAsync<TValue>(
        string topic,
        int partition,
        string? key,
        TValue value,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flush pending messages
    /// </summary>
    Task FlushAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a produce operation
/// </summary>
public sealed record ProduceResult
{
    public required string Topic { get; init; }
    public required int Partition { get; init; }
    public required long Offset { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public string? Key { get; init; }
}
