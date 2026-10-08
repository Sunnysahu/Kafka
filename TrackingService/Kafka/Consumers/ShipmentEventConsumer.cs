using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using System.Text.Json;
using TrackingService.Data;
using TrackingService.Models;

namespace TrackingService.Kafka.Consumers;

public class ShipmentEventConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ShipmentEventConsumer> _logger;

    public ShipmentEventConsumer(IConfiguration configuration, 
        ILogger<ShipmentEventConsumer> logger, IServiceScopeFactory scopeFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = _configuration["Kafka:ConsumerGroup"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
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


                using var scope = _scopeFactory.CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<TrackingDbContext>();    


                var tracking = new ShipmentTracking
                {
                    ShipmentId = shipmentEvent.ShipmentId,
                    Status = "Created",
                    CreatedAt = shipmentEvent.CreatedAt,
                    UpdatedAt = DateTime.Now
                };


                dbContext.ShipmentTracking.Add(tracking);

                await dbContext.SaveChangesAsync(stoppingToken);

                _logger.LogInformation(
                    "Received ShipmentCreated event. " + "ShipmentId: {ShipmentId}, Partition: {Partition}, Offset: {Offset}",
                    shipmentEvent.ShipmentId,
                    result.Partition,
                    result.Offset
                );

                consumer.Commit(result);
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