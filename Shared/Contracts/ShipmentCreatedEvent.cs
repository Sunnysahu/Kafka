namespace Shared.Contracts;

public class ShipmentCreatedEvent
{
    public int ShipmentId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string Origin { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
