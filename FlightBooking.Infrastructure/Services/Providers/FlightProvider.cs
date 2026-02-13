using FlightBooking.Application.DTOs;
using FlightBooking.Application.Interfaces;
using FlightBooking.Infrastructure.AirSearch;

namespace FlightBooking.Infrastructure.Services.Providers;

public class AirSearchFlightProvider : IFlightProvider
{
    private readonly IAirSearch _airSearchClient;

    public AirSearchFlightProvider(IAirSearch airSearchClient)
    {
        _airSearchClient = airSearchClient;
    }

    public async Task<List<FlightDto>> GetFlightsAsync(SearchRequestDto request)
    {
        var soapRequest = new SearchRequest
        {
            Origin = request.Origin,
            Destination = request.Destination,
            DepartureDate = request.DepartureDate
        };

        var soapResponse = await _airSearchClient.AvailabilitySearchAsync(soapRequest);
        
        if (soapResponse?.FlightOptions == null) return new List<FlightDto>();

        return soapResponse.FlightOptions.Select(item => new FlightDto
        {
            FlightNumber = item.FlightNumber,
            Price = item.Price,
            DepartureTime = item.DepartureDateTime,
            ArrivalTime = item.ArrivalDateTime
        }).ToList();
    }
}