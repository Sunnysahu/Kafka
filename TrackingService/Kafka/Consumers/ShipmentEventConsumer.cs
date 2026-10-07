using Confluent.Kafka;
using Shared.Contracts;
using System.Text.Json;

namespace TrackingService.Kafka.Consumers;

public class ShipmentEventConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ShipmentEventConsumer> _logger;

    public ShipmentEventConsumer(IConfiguration configuration, ILogger<ShipmentEventConsumer> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = _configuration["Kafka:ConsumerGroup"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        consumer.Subscribe(_configuration["Kafka:Topic"]);

        _logger.LogInformation("TrackingService Kafka consumer started.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);

                if (result?.Message == null)
                    continue;

                var shipmentEvent = JsonSerializer.Deserialize<ShipmentCreatedEvent>(result.Message.Value);

                if (shipmentEvent == null) continue;

                _logger.LogInformation(
                    "Received ShipmentCreated event. " +
                    "ShipmentId: {ShipmentId}, Partition: {Partition}, Offset: {Offset}",
                    shipmentEvent.ShipmentId,
                    result.Partition,
                    result.Offset);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("TrackingService Kafka consumer stopping.");
        }
        finally
        {
            consumer.Close();
        }
    }
}