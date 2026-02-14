using FlightBooking.Web.Models;

namespace FlightBooking.Web.Services;

public interface IFlightApiService
{
    Task<List<FlightDto>> SearchFlightsAsync(string origin, string destination, DateTime departureDate, DateTime? returnDate);
    Task<FlightDto?> GetFlightByIdAsync(string flightId);
    Task<List<AirportDto>> GetAirportsAsync();
}
