using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Kafka.Abstractions;
using Common.Kafka.Configuration;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Kafka.Implementations;

/// <summary>
/// Kafka producer implementation using Confluent.Kafka
/// </summary>
public sealed class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    public KafkaProducer(IOptions<KafkaOptions> options, ILogger<KafkaProducer> logger)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            SecurityProtocol = Enum.Parse<SecurityProtocol>(options.Value.SecurityProtocol, true),
            Acks = (Acks)options.Value.Producer.Acks,
            EnableIdempotence = options.Value.Producer.EnableIdempotence,
            MaxInFlight = options.Value.Producer.MaxInFlightRequestsPerConnection,
            MessageTimeoutMs = options.Value.Producer.MessageTimeoutMs,
            BatchSize = options.Value.Producer.BatchSize,
            LingerMs = options.Value.Producer.LingerMs,
            CompressionType = Enum.Parse<CompressionType>(options.Value.Producer.CompressionType, true),
            MessageSendMaxRetries = options.Value.Producer.Retries,
            RetryBackoffMs = options.Value.Producer.RetryBackoffMs,
            ClientId = $"{Environment.MachineName}-producer"
        };

        ConfigureSecurity(config, options.Value);

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, error) =>
            {
                _logger.LogError("Kafka producer error: {Code} - {Reason}", error.Code, error.Reason);
            })
            .SetLogHandler((_, log) =>
            {
                var level = log.Level switch
                {
                    SyslogLevel.Emergency or SyslogLevel.Alert or SyslogLevel.Critical => LogLevel.Critical,
                    SyslogLevel.Error => LogLevel.Error,
                    SyslogLevel.Warning => LogLevel.Warning,
                    SyslogLevel.Notice or SyslogLevel.Info => LogLevel.Information,
                    _ => LogLevel.Debug
                };
                _logger.Log(level, "Kafka: {Message}", log.Message);
            })
            .Build();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task<Abstractions.ProduceResult> ProduceAsync<TValue>(
        string topic,
        string? key,
        TValue value,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
        var message = new Message<string, string>
        {
            Key = key ?? Guid.NewGuid().ToString(),
            Value = serializedValue,
            Headers = CreateHeaders(headers)
        };

        var result = await _producer.ProduceAsync(topic, message, cancellationToken);

        _logger.LogDebug(
            "Produced message to {Topic}[{Partition}]@{Offset}",
            result.Topic, result.Partition.Value, result.Offset.Value);

        return new Abstractions.ProduceResult
        {
            Topic = result.Topic,
            Partition = result.Partition.Value,
            Offset = result.Offset.Value,
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(result.Timestamp.UnixTimestampMs),
            Key = result.Key
        };
    }

    public async Task<Abstractions.ProduceResult> ProduceAsync<TValue>(
        string topic,
        int partition,
        string? key,
        TValue value,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
        var message = new Message<string, string>
        {
            Key = key ?? Guid.NewGuid().ToString(),
            Value = serializedValue,
            Headers = CreateHeaders(headers)
        };

        var topicPartition = new Confluent.Kafka.TopicPartition(topic, new Partition(partition));
        var result = await _producer.ProduceAsync(topicPartition, message, cancellationToken);

        _logger.LogDebug(
            "Produced message to {Topic}[{Partition}]@{Offset}",
            result.Topic, result.Partition.Value, result.Offset.Value);

        return new Abstractions.ProduceResult
        {
            Topic = result.Topic,
            Partition = result.Partition.Value,
            Offset = result.Offset.Value,
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(result.Timestamp.UnixTimestampMs),
            Key = result.Key
        };
    }

    public Task FlushAsync(CancellationToken cancellationToken = default)
    {
        _producer.Flush(cancellationToken);
        return Task.CompletedTask;
    }

    private static Headers? CreateHeaders(IDictionary<string, string>? headers)
    {
        if (headers is null || headers.Count == 0)
            return null;

        var kafkaHeaders = new Headers();
        foreach (var (key, value) in headers)
        {
            kafkaHeaders.Add(key, System.Text.Encoding.UTF8.GetBytes(value));
        }
        return kafkaHeaders;
    }

    private static void ConfigureSecurity(ProducerConfig config, KafkaOptions options)
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

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        await FlushAsync();
        _producer.Dispose();
        _disposed = true;
    }
}
