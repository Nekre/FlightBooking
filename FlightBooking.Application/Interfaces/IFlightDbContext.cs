using FlightBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace FlightBooking.Application.Interfaces;

public interface IFlightDbContext
{
    DbSet<Airport> Airports { get; }
}