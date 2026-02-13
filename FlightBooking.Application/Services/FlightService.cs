using FlightBooking.Application.DTOs;
using FlightBooking.Application.Interfaces;

namespace FlightBooking.Application.Services;

public class FlightSearchService : IFlightService
{
    private readonly IEnumerable<IFlightProvider> _providers;
    private readonly ICacheService _cacheService;

    public FlightSearchService(IEnumerable<IFlightProvider> providers, ICacheService cacheService)
    {
        _providers = providers;
        _cacheService = cacheService;
    }

    public async Task<List<FlightDto>> SearchAsync(SearchRequestDto request)
    {
        string cacheKey = $"Search_{request.Origin}_{request.Destination}_{request.DepartureDate:yyyyMMdd}";

        var cachedResults = await _cacheService.GetAsync<List<FlightDto>>(cacheKey);
        if (cachedResults != null) return cachedResults;

        var tasks = _providers.Select(p => p.GetFlightsAsync(request));
        var resultsArray = await Task.WhenAll(tasks);

        var allFlights = resultsArray.SelectMany(f => f).ToList();

        if (allFlights.Any())
        {
            await _cacheService.SetAsync(cacheKey, allFlights, TimeSpan.FromMinutes(10));
        }

        return allFlights;
    }
}