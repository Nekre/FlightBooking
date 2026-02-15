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

    public async Task<SearchResponseDto> SearchAsync(SearchRequestDto request)
    {
        var cacheKey = CacheKeys.SearchKey(request);
        var cachedResponse = await _cacheService.GetAsync<SearchResponseDto>(cacheKey);
        if (cachedResponse != null)
        {
            cachedResponse.FromCache = true;
            return cachedResponse;
        }

        var outboundFlights = await SearchFlightsFromProvidersAsync(request);

        var inboundFlights = new List<FlightDto>();
        var inboundRequest = request.CreateInboundRequest();
        if (inboundRequest != null)
        {
            inboundFlights = await SearchFlightsFromProvidersAsync(inboundRequest);
        }

        EnrichFlights(outboundFlights, request);
        EnrichFlights(inboundFlights, inboundRequest);

        var allFlights = outboundFlights.Concat(inboundFlights).ToList();

        if (allFlights.Any())
        {
            await CacheFlightsAsync(allFlights, cacheKey);
        }

        return BuildSearchResponse(outboundFlights, inboundFlights);
    }


    private async Task<List<FlightDto>> SearchFlightsFromProvidersAsync(SearchRequestDto request)
    {
        var tasks = _providers.Select(p => p.GetFlightsAsync(request));
        var resultsArray = await Task.WhenAll(tasks);
        return resultsArray.SelectMany(f => f).ToList();
    }
    
    private void EnrichFlights(List<FlightDto> flights, SearchRequestDto request)
    {
        foreach (var flight in flights)
        {
            EnrichFlight(flight, request);
        }
    }

    private void EnrichFlight(FlightDto flight, SearchRequestDto request)
    {
        flight.Id = $"{flight.FlightNumber}_{flight.DepartureTime:yyyyMMddHHmm}";
    }
    
    private async Task CacheFlightsAsync(List<FlightDto> flights, string searchCacheKey)
    {
        var flightCacheTasks = flights.Select(flight =>
            _cacheService.SetAsync(
                CacheKeys.FlightKey(flight.Id),
                flight,
                SearchCacheTtl
            )
        );

        await Task.WhenAll(flightCacheTasks);

        await _cacheService.SetAsync(searchCacheKey, flights, SearchCacheTtl);
    }
    
    private SearchResponseDto BuildSearchResponse(List<FlightDto> outboundFlights, List<FlightDto> inboundFlights)
    {
        return new SearchResponseDto
        {
            OutboundFlights = outboundFlights,
            InboundFlights = inboundFlights,
            Success = true,
            FromCache = false,
        };
    }
}
