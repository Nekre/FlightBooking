using FlightBooking.Application.DTOs;

namespace FlightBooking.Application.Interfaces;

public interface IFlightService
{
    Task<List<FlightDto>> SearchAsync(SearchRequestDto request);
}