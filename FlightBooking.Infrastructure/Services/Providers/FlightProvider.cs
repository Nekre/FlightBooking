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

    public async Task<List<FlightDto>> GetFlightsAsync(SearchRequestDto request,CancellationToken cancellationToken)
    {
        var soapRequest = new SearchRequest
        {
            Origin = request.Origin,
            Destination = request.Destination,
            DepartureDate = request.DepartureDate
        };

        var soapTask = _airSearchClient.AvailabilitySearchAsync(soapRequest);

        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(50), cancellationToken);

        var completedTask = await Task.WhenAny(soapTask, timeoutTask);

        if (completedTask != soapTask)
            throw new TimeoutException("AirSearch provider timeout (50s).");

        var soapResponse = await soapTask;

        if (soapResponse?.FlightOptions == null)
            return new List<FlightDto>();

        return soapResponse.FlightOptions
            .Select(item => new FlightDto
            {
                FlightNumber = item.FlightNumber,
                Origin = request.Origin,
                Destination = request.Destination,
                Price = item.Price,
                DepartureTime = item.DepartureDateTime,
                ArrivalTime = item.ArrivalDateTime
            })
            .ToList();
    }
}