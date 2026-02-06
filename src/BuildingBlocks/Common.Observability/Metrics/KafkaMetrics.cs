using System.Diagnostics.Metrics;

namespace Common.Observability.Metrics;

/// <summary>
/// Kafka-specific metrics
/// </summary>
public sealed class KafkaMetrics : IDisposable
{
    private readonly Meter _meter;

    public KafkaMetrics()
    {
        _meter = new Meter("PosWms.Kafka", "1.0.0");

        MessagesProduced = _meter.CreateCounter<long>(
            "kafka_messages_produced_total",
            description: "Total number of messages produced to Kafka");

        MessagesConsumed = _meter.CreateCounter<long>(
            "kafka_messages_consumed_total",
            description: "Total number of messages consumed from Kafka");

        ProduceErrors = _meter.CreateCounter<long>(
            "kafka_produce_errors_total",
            description: "Total number of produce errors");

        ConsumeErrors = _meter.CreateCounter<long>(
            "kafka_consume_errors_total",
            description: "Total number of consume errors");

        ProduceLatency = _meter.CreateHistogram<double>(
            "kafka_produce_latency_seconds",
            unit: "s",
            description: "Message produce latency in seconds");

        ConsumeLatency = _meter.CreateHistogram<double>(
            "kafka_consume_latency_seconds",
            unit: "s",
            description: "End-to-end message latency in seconds");

        BatchSize = _meter.CreateHistogram<int>(
            "kafka_batch_size",
            description: "Number of messages in a batch");

        ConsumerLag = _meter.CreateObservableGauge(
            "kafka_consumer_lag",
            () => _lagMeasurements,
            description: "Consumer lag per partition");
    }

    public Counter<long> MessagesProduced { get; }
    public Counter<long> MessagesConsumed { get; }
    public Counter<long> ProduceErrors { get; }
    public Counter<long> ConsumeErrors { get; }
    public Histogram<double> ProduceLatency { get; }
    public Histogram<double> ConsumeLatency { get; }
    public Histogram<int> BatchSize { get; }
    public ObservableGauge<long> ConsumerLag { get; private set; } = null!;

    private readonly List<Measurement<long>> _lagMeasurements = new();

    public void RecordProduced(string topic, int partition)
    {
        MessagesProduced.Add(1, new("topic", topic), new("partition", partition.ToString()));
    }

    public void RecordConsumed(string topic, int partition, string consumerGroup)
    {
        MessagesConsumed.Add(1,
            new("topic", topic),
            new("partition", partition.ToString()),
            new("consumer_group", consumerGroup));
    }

    public void RecordProduceError(string topic, string errorCode)
    {
        ProduceErrors.Add(1, new("topic", topic), new("error_code", errorCode));
    }

    public void RecordConsumeError(string topic, string consumerGroup, string errorCode)
    {
        ConsumeErrors.Add(1,
            new("topic", topic),
            new("consumer_group", consumerGroup),
            new("error_code", errorCode));
    }

    public void UpdateConsumerLag(string topic, int partition, string consumerGroup, long lag)
    {
        _lagMeasurements.RemoveAll(m =>
            m.Tags.ToArray().Any(t => t.Key == "topic" && (string)t.Value! == topic) &&
            m.Tags.ToArray().Any(t => t.Key == "partition" && (string)t.Value! == partition.ToString()));

        _lagMeasurements.Add(new Measurement<long>(lag,
            new("topic", topic),
            new("partition", partition.ToString()),
            new("consumer_group", consumerGroup)));
    }

    public void Dispose()
    {
        _meter.Dispose();
    }
}
