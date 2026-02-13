using FlightBooking.Application.DTOs;
using FlightBooking.Application.Interfaces;
using MediatR;

namespace FlightBooking.Application.Features.Flights.Queries.GetById;

public class GetFlightByIdHandler : IRequestHandler<GetFlightByIdQuery, FlightDto?>
{
    private readonly ICacheService _cacheService;

    public GetFlightByIdHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<FlightDto?> Handle(GetFlightByIdQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = $"Flight_{request.FlightId}";
        var flight = await _cacheService.GetAsync<FlightDto>(cacheKey, cancellationToken);
        
        return flight;
    }
}
