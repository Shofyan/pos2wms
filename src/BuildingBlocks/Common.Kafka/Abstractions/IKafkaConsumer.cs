namespace Common.Kafka.Abstractions;

/// <summary>
/// Interface for Kafka consumer
/// </summary>
public interface IKafkaConsumer : IAsyncDisposable
{
    /// <summary>
    /// Subscribe to topics
    /// </summary>
    void Subscribe(IEnumerable<string> topics);

    /// <summary>
    /// Subscribe to a single topic
    /// </summary>
    void Subscribe(string topic);

    /// <summary>
    /// Consume a message
    /// </summary>
    Task<ConsumeResult<TValue>?> ConsumeAsync<TValue>(
        TimeSpan timeout,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit the current offsets
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit specific message offset
    /// </summary>
    Task CommitAsync(ConsumeResult<object> result, CancellationToken cancellationToken = default);

    /// <summary>
    /// Seek to a specific offset
    /// </summary>
    void Seek(string topic, int partition, long offset);

    /// <summary>
    /// Pause consumption from partitions
    /// </summary>
    void Pause(IEnumerable<TopicPartition> partitions);

    /// <summary>
    /// Resume consumption from partitions
    /// </summary>
    void Resume(IEnumerable<TopicPartition> partitions);

    /// <summary>
    /// Get current assignment
    /// </summary>
    IReadOnlyList<TopicPartition> Assignment { get; }

    /// <summary>
    /// Close the consumer
    /// </summary>
    void Close();
}

/// <summary>
/// Result of a consume operation
/// </summary>
public sealed record ConsumeResult<TValue>
{
    public required string Topic { get; init; }
    public required int Partition { get; init; }
    public required long Offset { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public string? Key { get; init; }
    public required TValue Value { get; init; }
    public IReadOnlyDictionary<string, string>? Headers { get; init; }
    public bool IsPartitionEof { get; init; }
}

/// <summary>
/// Topic partition identifier
/// </summary>
public sealed record TopicPartition(string Topic, int Partition);
