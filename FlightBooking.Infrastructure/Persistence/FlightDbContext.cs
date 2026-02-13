using FlightBooking.Domain.Entities;
using System.Reflection;
using FlightBooking.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Infrastructure.Persistence;

public class FlightDbContext : DbContext,IFlightDbContext
{
    public FlightDbContext(DbContextOptions<FlightDbContext> options) : base(options)
    {
    }

    public DbSet<Airport> Airports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
    }
}