using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Common.Observability.Metrics;

/// <summary>
/// Central metrics registry for the application
/// </summary>
public sealed class MetricsRegistry : IDisposable
{
    private readonly Meter _meter;

    public MetricsRegistry(string serviceName, string? serviceVersion = null)
    {
        _meter = new Meter(serviceName, serviceVersion ?? "1.0.0");

        // Request metrics
        RequestsTotal = _meter.CreateCounter<long>(
            "pos_wms_requests_total",
            description: "Total number of HTTP requests");

        RequestDuration = _meter.CreateHistogram<double>(
            "pos_wms_request_duration_seconds",
            unit: "s",
            description: "HTTP request duration in seconds");

        // Business metrics
        SalesTotal = _meter.CreateCounter<long>(
            "pos_sales_total",
            description: "Total number of sales transactions");

        SalesAmount = _meter.CreateCounter<decimal>(
            "pos_sales_amount_total",
            unit: "currency",
            description: "Total sales amount");

        ReturnsTotal = _meter.CreateCounter<long>(
            "pos_returns_total",
            description: "Total number of return transactions");

        InventoryAdjustments = _meter.CreateCounter<long>(
            "wms_inventory_adjustments_total",
            description: "Total number of inventory adjustments");

        // Event metrics
        EventsPublished = _meter.CreateCounter<long>(
            "pos_wms_events_published_total",
            description: "Total number of events published");

        EventsConsumed = _meter.CreateCounter<long>(
            "pos_wms_events_consumed_total",
            description: "Total number of events consumed");

        EventsFailedTotal = _meter.CreateCounter<long>(
            "pos_wms_events_failed_total",
            description: "Total number of failed events");

        EventProcessingDuration = _meter.CreateHistogram<double>(
            "pos_wms_event_processing_duration_seconds",
            unit: "s",
            description: "Event processing duration in seconds");

        // Infrastructure metrics
        ActiveConnections = _meter.CreateUpDownCounter<int>(
            "pos_wms_active_connections",
            description: "Number of active database connections");

        KafkaConsumerLag = _meter.CreateObservableGauge(
            "pos_wms_kafka_consumer_lag",
            () => _consumerLag,
            description: "Kafka consumer lag in messages");
    }

    // Request metrics
    public Counter<long> RequestsTotal { get; }
    public Histogram<double> RequestDuration { get; }

    // Business metrics
    public Counter<long> SalesTotal { get; }
    public Counter<decimal> SalesAmount { get; }
    public Counter<long> ReturnsTotal { get; }
    public Counter<long> InventoryAdjustments { get; }

    // Event metrics
    public Counter<long> EventsPublished { get; }
    public Counter<long> EventsConsumed { get; }
    public Counter<long> EventsFailedTotal { get; }
    public Histogram<double> EventProcessingDuration { get; }

    // Infrastructure metrics
    public UpDownCounter<int> ActiveConnections { get; }
    public ObservableGauge<long> KafkaConsumerLag { get; private set; } = null!;

    private long _consumerLag;
    public void SetConsumerLag(long lag) => _consumerLag = lag;

    /// <summary>
    /// Create a stopwatch for timing operations
    /// </summary>
    public static Stopwatch StartTimer() => Stopwatch.StartNew();

    /// <summary>
    /// Record a request with duration
    /// </summary>
    public void RecordRequest(string method, string path, int statusCode, double durationSeconds)
    {
        var tags = new TagList
        {
            { "method", method },
            { "path", path },
            { "status_code", statusCode.ToString() }
        };

        RequestsTotal.Add(1, tags);
        RequestDuration.Record(durationSeconds, tags);
    }

    /// <summary>
    /// Record a sale
    /// </summary>
    public void RecordSale(string storeId, decimal amount, string currency)
    {
        var tags = new TagList
        {
            { "store_id", storeId },
            { "currency", currency }
        };

        SalesTotal.Add(1, tags);
        SalesAmount.Add(amount, tags);
    }

    public void Dispose()
    {
        _meter.Dispose();
    }
}
