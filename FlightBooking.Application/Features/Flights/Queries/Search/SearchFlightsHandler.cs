using MediatR;
using FlightBooking.Application.DTOs;
using FlightBooking.Application.Features.Flights.Queries.Search;
using FlightBooking.Application.Interfaces;

namespace FlightBooking.Application.Features.Flights.Queries.SearchFlights;

public class SearchFlightsHandler : IRequestHandler<SearchFlightsQuery, SearchResponseDto>
{
    private readonly IFlightService _flightService;

    public SearchFlightsHandler(IFlightService flightService)
    {
        _flightService = flightService;
    }

    public async Task<SearchResponseDto> Handle(SearchFlightsQuery request, CancellationToken cancellationToken)
    {
        var searchRequest = new SearchRequestDto
        {
            Origin = request.Origin,
            Destination = request.Destination,
            DepartureDate = request.DepartureDate,
            ReturnDate = request.ReturnDate
        };

        return await _flightService.SearchAsync(searchRequest);
    }
}