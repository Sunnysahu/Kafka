namespace Shared.Contracts;

public class ShipmentCreatedEvent
{
    public Guid EventId { get; set; }

    public int ShipmentId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string Origin { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
