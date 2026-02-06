using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Kafka.Configuration;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ConsumeResultAbstraction = Common.Kafka.Abstractions.ConsumeResult<object>;
using TopicPartitionAbstraction = Common.Kafka.Abstractions.TopicPartition;

namespace Common.Kafka.Implementations;

/// <summary>
/// Kafka consumer implementation using Confluent.Kafka
/// </summary>
public sealed class KafkaConsumer : Abstractions.IKafkaConsumer
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    public KafkaConsumer(IOptions<KafkaOptions> options, ILogger<KafkaConsumer> logger)
    {
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            SecurityProtocol = Enum.Parse<SecurityProtocol>(options.Value.SecurityProtocol, true),
            GroupId = options.Value.Consumer.GroupId,
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(options.Value.Consumer.AutoOffsetReset, true),
            EnableAutoCommit = options.Value.Consumer.EnableAutoCommit,
            AutoCommitIntervalMs = options.Value.Consumer.AutoCommitIntervalMs,
            SessionTimeoutMs = options.Value.Consumer.SessionTimeoutMs,
            HeartbeatIntervalMs = options.Value.Consumer.HeartbeatIntervalMs,
            MaxPollIntervalMs = options.Value.Consumer.MaxPollIntervalMs,
            FetchMinBytes = options.Value.Consumer.FetchMinBytes,
            FetchMaxBytes = options.Value.Consumer.FetchMaxBytes,
            EnablePartitionEof = options.Value.Consumer.EnablePartitionEof,
            PartitionAssignmentStrategy = Enum.Parse<PartitionAssignmentStrategy>(
                options.Value.Consumer.PartitionAssignmentStrategy.Replace("-", ""), true),
            ClientId = $"{Environment.MachineName}-consumer"
        };

        ConfigureSecurity(config, options.Value);

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, error) =>
            {
                _logger.LogError("Kafka consumer error: {Code} - {Reason}", error.Code, error.Reason);
            })
            .SetPartitionsAssignedHandler((consumer, partitions) =>
            {
                _logger.LogInformation(
                    "Partitions assigned: {Partitions}",
                    string.Join(", ", partitions.Select(p => $"{p.Topic}[{p.Partition}]")));
            })
            .SetPartitionsRevokedHandler((consumer, partitions) =>
            {
                _logger.LogInformation(
                    "Partitions revoked: {Partitions}",
                    string.Join(", ", partitions.Select(p => $"{p.Topic}[{p.Partition}]")));
            })
            .SetOffsetsCommittedHandler((consumer, offsets) =>
            {
                if (offsets.Error is not null && offsets.Error.Code != ErrorCode.NoError)
                {
                    _logger.LogWarning(
                        "Offset commit failed: {Error}",
                        offsets.Error.Reason);
                }
            })
            .Build();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public IReadOnlyList<TopicPartitionAbstraction> Assignment =>
        _consumer.Assignment
            .Select(tp => new TopicPartitionAbstraction(tp.Topic, tp.Partition.Value))
            .ToList();

    public void Subscribe(IEnumerable<string> topics)
    {
        _consumer.Subscribe(topics);
        _logger.LogInformation("Subscribed to topics: {Topics}", string.Join(", ", topics));
    }

    public void Subscribe(string topic)
    {
        _consumer.Subscribe(topic);
        _logger.LogInformation("Subscribed to topic: {Topic}", topic);
    }

    public Task<Abstractions.ConsumeResult<TValue>?> ConsumeAsync<TValue>(
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = _consumer.Consume(timeout);
            if (result is null)
                return Task.FromResult<Abstractions.ConsumeResult<TValue>?>(null);

            if (result.IsPartitionEOF)
            {
                return Task.FromResult<Abstractions.ConsumeResult<TValue>?>(new Abstractions.ConsumeResult<TValue>
                {
                    Topic = result.Topic,
                    Partition = result.Partition.Value,
                    Offset = result.Offset.Value,
                    Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(result.Message?.Timestamp.UnixTimestampMs ?? 0),
                    Key = result.Message?.Key,
                    Value = default!,
                    IsPartitionEof = true
                });
            }

            var value = JsonSerializer.Deserialize<TValue>(result.Message.Value, _jsonOptions)!;
            var headers = result.Message.Headers?
                .ToDictionary(
                    h => h.Key,
                    h => System.Text.Encoding.UTF8.GetString(h.GetValueBytes()));

            return Task.FromResult<Abstractions.ConsumeResult<TValue>?>(new Abstractions.ConsumeResult<TValue>
            {
                Topic = result.Topic,
                Partition = result.Partition.Value,
                Offset = result.Offset.Value,
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(result.Message.Timestamp.UnixTimestampMs),
                Key = result.Message.Key,
                Value = value,
                Headers = headers
            });
        }
        catch (ConsumeException ex)
        {
            _logger.LogError(ex, "Error consuming message: {Reason}", ex.Error.Reason);
            throw;
        }
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        _consumer.Commit();
        return Task.CompletedTask;
    }

    public Task CommitAsync(ConsumeResultAbstraction result, CancellationToken cancellationToken = default)
    {
        var topicPartitionOffset = new TopicPartitionOffset(
            result.Topic,
            new Partition(result.Partition),
            new Offset(result.Offset + 1));

        _consumer.Commit(new[] { topicPartitionOffset });
        return Task.CompletedTask;
    }

    public void Seek(string topic, int partition, long offset)
    {
        var topicPartitionOffset = new TopicPartitionOffset(
            topic,
            new Partition(partition),
            new Offset(offset));

        _consumer.Seek(topicPartitionOffset);
    }

    public void Pause(IEnumerable<TopicPartitionAbstraction> partitions)
    {
        var topicPartitions = partitions
            .Select(p => new TopicPartition(p.Topic, new Partition(p.Partition)))
            .ToList();

        _consumer.Pause(topicPartitions);
    }

    public void Resume(IEnumerable<TopicPartitionAbstraction> partitions)
    {
        var topicPartitions = partitions
            .Select(p => new TopicPartition(p.Topic, new Partition(p.Partition)))
            .ToList();

        _consumer.Resume(topicPartitions);
    }

    public void Close()
    {
        _consumer.Close();
        _logger.LogInformation("Consumer closed");
    }

    private static void ConfigureSecurity(ConsumerConfig config, KafkaOptions options)
    {
        if (!string.IsNullOrEmpty(options.SaslMechanism))
        {
            config.SaslMechanism = Enum.Parse<SaslMechanism>(options.SaslMechanism, true);
            config.SaslUsername = options.SaslUsername;
            config.SaslPassword = options.SaslPassword;
        }

        if (!string.IsNullOrEmpty(options.SslCaLocation))
        {
            config.SslCaLocation = options.SslCaLocation;
        }

        if (!string.IsNullOrEmpty(options.SslCertificateLocation))
        {
            config.SslCertificateLocation = options.SslCertificateLocation;
            config.SslKeyLocation = options.SslKeyLocation;
        }
    }

    public ValueTask DisposeAsync()
    {
        if (_disposed) return ValueTask.CompletedTask;

        Close();
        _consumer.Dispose();
        _disposed = true;
        return ValueTask.CompletedTask;
    }
}
