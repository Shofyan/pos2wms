namespace Common.Kafka.Configuration;

/// <summary>
/// Kafka configuration options
/// </summary>
public sealed class KafkaOptions
{
    public const string SectionName = "Kafka";

    /// <summary>
    /// Bootstrap servers (comma-separated)
    /// </summary>
    public string BootstrapServers { get; set; } = "localhost:9092";

    /// <summary>
    /// Security protocol (PLAINTEXT, SSL, SASL_PLAINTEXT, SASL_SSL)
    /// </summary>
    public string SecurityProtocol { get; set; } = "PLAINTEXT";

    /// <summary>
    /// SASL mechanism (PLAIN, SCRAM-SHA-256, SCRAM-SHA-512)
    /// </summary>
    public string? SaslMechanism { get; set; }

    /// <summary>
    /// SASL username
    /// </summary>
    public string? SaslUsername { get; set; }

    /// <summary>
    /// SASL password
    /// </summary>
    public string? SaslPassword { get; set; }

    /// <summary>
    /// SSL CA certificate path
    /// </summary>
    public string? SslCaLocation { get; set; }

    /// <summary>
    /// SSL client certificate path
    /// </summary>
    public string? SslCertificateLocation { get; set; }

    /// <summary>
    /// SSL client key path
    /// </summary>
    public string? SslKeyLocation { get; set; }

    /// <summary>
    /// Schema Registry URL
    /// </summary>
    public string? SchemaRegistryUrl { get; set; }

    /// <summary>
    /// Producer specific options
    /// </summary>
    public KafkaProducerOptions Producer { get; set; } = new();

    /// <summary>
    /// Default consumer options
    /// </summary>
    public KafkaConsumerOptions Consumer { get; set; } = new();
}

/// <summary>
/// Kafka producer configuration options
/// </summary>
public sealed class KafkaProducerOptions
{
    /// <summary>
    /// Acks: 0 = none, 1 = leader, -1 = all
    /// </summary>
    public int Acks { get; set; } = -1;

    /// <summary>
    /// Enable idempotent producer
    /// </summary>
    public bool EnableIdempotence { get; set; } = true;

    /// <summary>
    /// Maximum in-flight requests per connection
    /// </summary>
    public int MaxInFlightRequestsPerConnection { get; set; } = 5;

    /// <summary>
    /// Message timeout in milliseconds
    /// </summary>
    public int MessageTimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Batch size in bytes
    /// </summary>
    public int BatchSize { get; set; } = 16384;

    /// <summary>
    /// Linger in milliseconds
    /// </summary>
    public int LingerMs { get; set; } = 5;

    /// <summary>
    /// Compression type (none, gzip, snappy, lz4, zstd)
    /// </summary>
    public string CompressionType { get; set; } = "snappy";

    /// <summary>
    /// Number of retries
    /// </summary>
    public int Retries { get; set; } = 3;

    /// <summary>
    /// Retry backoff in milliseconds
    /// </summary>
    public int RetryBackoffMs { get; set; } = 100;
}

/// <summary>
/// Kafka consumer configuration options
/// </summary>
public sealed class KafkaConsumerOptions
{
    /// <summary>
    /// Consumer group ID
    /// </summary>
    public string GroupId { get; set; } = "default-consumer-group";

    /// <summary>
    /// Auto offset reset (earliest, latest, error)
    /// </summary>
    public string AutoOffsetReset { get; set; } = "earliest";

    /// <summary>
    /// Enable auto commit
    /// </summary>
    public bool EnableAutoCommit { get; set; } = false;

    /// <summary>
    /// Auto commit interval in milliseconds
    /// </summary>
    public int AutoCommitIntervalMs { get; set; } = 5000;

    /// <summary>
    /// Session timeout in milliseconds
    /// </summary>
    public int SessionTimeoutMs { get; set; } = 45000;

    /// <summary>
    /// Heartbeat interval in milliseconds
    /// </summary>
    public int HeartbeatIntervalMs { get; set; } = 3000;

    /// <summary>
    /// Maximum poll interval in milliseconds
    /// </summary>
    public int MaxPollIntervalMs { get; set; } = 300000;

    /// <summary>
    /// Maximum number of messages per poll
    /// </summary>
    public int MaxPollRecords { get; set; } = 500;

    /// <summary>
    /// Fetch minimum bytes
    /// </summary>
    public int FetchMinBytes { get; set; } = 1;

    /// <summary>
    /// Fetch maximum bytes
    /// </summary>
    public int FetchMaxBytes { get; set; } = 52428800;

    /// <summary>
    /// Partition assignment strategy (range, roundrobin, sticky, cooperative-sticky)
    /// </summary>
    public string PartitionAssignmentStrategy { get; set; } = "cooperative-sticky";

    /// <summary>
    /// Enable partition EOF notification
    /// </summary>
    public bool EnablePartitionEof { get; set; } = false;
}
