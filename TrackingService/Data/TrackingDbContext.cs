using Microsoft.EntityFrameworkCore;
using TrackingService.Models;

namespace TrackingService.Data;


public class TrackingDbContext(DbContextOptions<TrackingDbContext> options) : DbContext(options)
{
    public DbSet<ShipmentTracking> ShipmentTracking => Set<ShipmentTracking>();
}