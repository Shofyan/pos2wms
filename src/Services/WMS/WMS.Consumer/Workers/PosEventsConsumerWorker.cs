using Common.Events.Models;
using Common.Kafka.Abstractions;
using Microsoft.Extensions.Options;
using WMS.Consumer.Handlers;

namespace WMS.Consumer.Workers;

/// <summary>
/// Background worker for consuming POS events
/// </summary>
public sealed class PosEventsConsumerWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PosEventsConsumerWorker> _logger;
    private readonly ConsumerOptions _options;

    public PosEventsConsumerWorker(
        IServiceProvider serviceProvider,
        ILogger<PosEventsConsumerWorker> logger,
        IOptions<ConsumerOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("POS Events Consumer Worker starting...");

        await Task.Delay(5000, stoppingToken); // Wait for Kafka to be ready

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConsumeMessagesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("POS Events Consumer Worker stopping...");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in POS Events Consumer Worker. Retrying in 5 seconds...");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    private async Task ConsumeMessagesAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var consumer = scope.ServiceProvider.GetRequiredService<IKafkaConsumer>();

        consumer.Subscribe(_options.Topics);

        _logger.LogInformation("Subscribed to topics: {Topics}", string.Join(", ", _options.Topics));

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await consumer.ConsumeAsync<string>(TimeSpan.FromSeconds(5), stoppingToken);

            if (result == null || result.Value == null)
            {
                continue;
            }

            _logger.LogDebug(
                "Received message from topic {Topic}, partition {Partition}, offset {Offset}",
                result.Topic, result.Partition, result.Offset);

            try
            {
                await ProcessMessageAsync(scope.ServiceProvider, result, stoppingToken);
                await consumer.CommitAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to process message from topic {Topic}, offset {Offset}",
                    result.Topic, result.Offset);

                // TODO: Implement dead letter queue
            }
        }
    }

    private async Task ProcessMessageAsync(
        IServiceProvider serviceProvider,
        ConsumeResult<string> result,
        CancellationToken cancellationToken)
    {
        var eventType = result.Headers?.TryGetValue("EventType", out var typeHeader) == true
            ? typeHeader
            : null;

        switch (eventType)
        {
            case "pos.sale.completed":
                var saleCompletedEvent = System.Text.Json.JsonSerializer.Deserialize<SaleCompletedEvent>(result.Value);
                if (saleCompletedEvent != null)
                {
                    var handler = serviceProvider.GetRequiredService<SaleCompletedEventHandler>();
                    await handler.HandleAsync(saleCompletedEvent, cancellationToken);
                }
                break;

            case "pos.sale.cancelled":
                var saleCancelledEvent = System.Text.Json.JsonSerializer.Deserialize<SaleCancelledEvent>(result.Value);
                if (saleCancelledEvent != null)
                {
                    var handler = serviceProvider.GetRequiredService<SaleCancelledEventHandler>();
                    await handler.HandleAsync(saleCancelledEvent, cancellationToken);
                }
                break;

            case "pos.return.created":
                var returnCreatedEvent = System.Text.Json.JsonSerializer.Deserialize<ReturnCreatedEvent>(result.Value);
                if (returnCreatedEvent != null)
                {
                    var handler = serviceProvider.GetRequiredService<ReturnCreatedEventHandler>();
                    await handler.HandleAsync(returnCreatedEvent, cancellationToken);
                }
                break;

            default:
                _logger.LogWarning("Unknown event type: {EventType}", eventType);
                break;
        }
    }
}

public sealed class ConsumerOptions
{
    public List<string> Topics { get; set; } = new();
}
