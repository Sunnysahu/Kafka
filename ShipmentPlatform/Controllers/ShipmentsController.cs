using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using ShipmentService.Kafka.Producers;

namespace ShipmentService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShipmentsController : ControllerBase
{
    private readonly KafkaProducer _kafkaProducer;

    public ShipmentsController(KafkaProducer kafkaProducer) => _kafkaProducer = kafkaProducer;

    [HttpPost("test-event")]
    public async Task<IActionResult> PublishTestEvent(CancellationToken cancellationToken)
    {
        //var shipmentId = Random.Shared.Next(1000, 9999);
        var shipmentId = 1234;

        var shipmentEvent = new ShipmentCreatedEvent
        {
            ShipmentId = shipmentId,
            CustomerName = "Sunny",
            Origin = "Jamshedpur",
            Destination = "Delhi",
            CreatedAt = DateTime.Now
        };

        await _kafkaProducer.PublishAsync(shipmentId.ToString(), shipmentEvent, cancellationToken);

        return Ok(new
        {
            Success = true,
            ShipmentId = shipmentId,
            Message = "ShipmentCreated event published to Kafka."
        });
    }
}

