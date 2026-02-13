using MediatR;
using FlightBooking.Application.DTOs;
using FlightBooking.Application.Features.Flights.Queries.Search;
using FlightBooking.Application.Interfaces;

namespace FlightBooking.Application.Features.Flights.Queries.SearchFlights;

public class SearchFlightsHandler : IRequestHandler<SearchFlightsQuery, List<FlightDto>>
{
    private readonly IEnumerable<IFlightProvider> _providers;
    private readonly ICacheService _cacheService;

    public SearchFlightsHandler(IEnumerable<IFlightProvider> providers, ICacheService cacheService)
    {
        _providers = providers;
        _cacheService = cacheService;
    }

    public async Task<List<FlightDto>> Handle(SearchFlightsQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = $"Search_{request.Origin}_{request.Destination}_{request.DepartureDate:yyyyMMdd}";

        var cachedResults = await _cacheService.GetAsync<List<FlightDto>>(cacheKey);
        if (cachedResults != null) return cachedResults;

        var searchRequest = new SearchRequestDto { 
            Origin = request.Origin, 
            Destination = request.Destination, 
            DepartureDate = request.DepartureDate 
        };

        var tasks = _providers.Select(p => p.GetFlightsAsync(searchRequest));
        var resultsArray = await Task.WhenAll(tasks);
        var allFlights = resultsArray.SelectMany(f => f).ToList();

        if (allFlights.Any())
        {
            await _cacheService.SetAsync(cacheKey, allFlights, TimeSpan.FromMinutes(10));
        }

        return allFlights;
    }
}