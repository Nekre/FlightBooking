using FlightBooking.Application.DTOs;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Utilities;

namespace FlightBooking.Application.Services;

public class FlightSearchService : IFlightService
{
    private static readonly TimeSpan SearchCacheTtl = TimeSpan.FromMinutes(10);
    private readonly IEnumerable<IFlightProvider> _providers;
    private readonly ICacheService _cacheService;

    public FlightSearchService(IEnumerable<IFlightProvider> providers, ICacheService cacheService)
    {
        _providers = providers;
        _cacheService = cacheService;
    }

    public async Task<List<FlightDto>> SearchAsync(SearchRequestDto request)
    {
        var cacheKey = CacheKeys.SearchKey(request);

        var cachedResults = await _cacheService.GetAsync<List<FlightDto>>(cacheKey);
        if (cachedResults != null) return cachedResults;

        var tasks = _providers.Select(p => p.GetFlightsAsync(request));
        var resultsArray = await Task.WhenAll(tasks);

        var allFlights = resultsArray.SelectMany(f => f).ToList();

        if (allFlights.Any())
        {
            foreach (var flight in allFlights)
            {
                flight.Id = $"{flight.FlightNumber}_{flight.DepartureTime:yyyyMMddHHmm}";
                flight.Origin = request.Origin;
                flight.Destination = request.Destination;

                string flightCacheKey = CacheKeys.FlightKey(flight.Id);
                await _cacheService.SetAsync(flightCacheKey, flight, SearchCacheTtl);
            }

            await _cacheService.SetAsync(cacheKey, allFlights, SearchCacheTtl);
        }

        return allFlights;
    }
}