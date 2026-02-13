using FlightBooking.Application.DTOs;

namespace FlightBooking.Application.Interfaces;

public interface IFlightProvider
{
    Task<List<FlightDto>> GetFlightsAsync(SearchRequestDto request);
}