using FlightBooking.Application.DTOs;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Utilities;
using Microsoft.Extensions.Logging;

namespace FlightBooking.Application.Services;

public class FlightSearchService : IFlightService
{
    private static readonly TimeSpan SearchCacheTtl = TimeSpan.FromMinutes(10);
    private readonly IEnumerable<IFlightProvider> _providers;
    private readonly ICacheService _cacheService;
    private readonly ILogger<FlightSearchService> _logger;

    public FlightSearchService(IEnumerable<IFlightProvider> providers, ICacheService cacheService, ILogger<FlightSearchService> logger)
    {
        _providers = providers;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<SearchResponseDto> SearchAsync(SearchRequestDto request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.SearchKey(request);
        var cachedResponse = await _cacheService.GetAsync<SearchResponseDto>(cacheKey,cancellationToken);
        if (cachedResponse != null)
        {
            cachedResponse.FromCache = true;
            return cachedResponse;
        }

        var inboundRequest = request.CreateInboundRequest();

        var outboundTask =
            SearchFlightsFromProvidersAsync(request, cancellationToken);

        Task<List<FlightDto>> inboundTask =
            inboundRequest != null
                ? SearchFlightsFromProvidersAsync(inboundRequest, cancellationToken)
                : Task.FromResult(new List<FlightDto>());

        await Task.WhenAll(outboundTask, inboundTask);

        var outboundFlights = await outboundTask;
        var inboundFlights = await inboundTask;


        EnrichFlights(outboundFlights, request);
        EnrichFlights(inboundFlights, inboundRequest);

        var allFlights = outboundFlights.Concat(inboundFlights).ToList();

        if (allFlights.Any())
        {
            _ =  CacheFlightsAsync(allFlights, cacheKey,cancellationToken);
        }

        return BuildSearchResponse(outboundFlights, inboundFlights);
    }


    private async Task<List<FlightDto>> SearchFlightsFromProvidersAsync(SearchRequestDto request, CancellationToken cancellationToken)
    {
        var tasks = _providers.Select(p => p.GetFlightsAsync(request, cancellationToken));
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
    
    private async Task CacheFlightsAsync(List<FlightDto> flights, string searchCacheKey, CancellationToken cancellationToken)
    {
        try
        {
            var flightCacheTasks = flights.Select(flight =>
                _cacheService.SetAsync(
                    CacheKeys.FlightKey(flight.Id),
                    flight,
                    SearchCacheTtl,
                    cancellationToken
                )
            );

            await Task.WhenAll(flightCacheTasks);

            await _cacheService.SetAsync(searchCacheKey, flights, SearchCacheTtl, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to write flight cache. SearchKey: {SearchKey}, FlightCount: {FlightCount}",
                searchCacheKey,
                flights.Count);
        }
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