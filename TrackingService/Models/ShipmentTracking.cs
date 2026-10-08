namespace TrackingService.Models;

public class ShipmentTracking
{
    public int Id { get; set; }

    public int ShipmentId { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}