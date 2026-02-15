using FlightBooking.Application.DTOs;

namespace FlightBooking.Application.Interfaces;

public interface IFlightService
{
    Task<SearchResponseDto> SearchAsync(SearchRequestDto request);
}