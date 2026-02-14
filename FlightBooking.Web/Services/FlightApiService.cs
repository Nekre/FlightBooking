using System.Net.Http.Json;
using FlightBooking.Web.Models;

namespace FlightBooking.Web.Services;

public class FlightApiService : IFlightApiService
{
    private readonly HttpClient _httpClient;

    public FlightApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<FlightDto>> SearchFlightsAsync(string origin, string destination, DateTime departureDate, DateTime? returnDate)
    {
        var returnDateParam = returnDate.HasValue ? $"&returnDate={returnDate:yyyy-MM-dd}" : string.Empty;
        var url = $"/Flights/search?origin={origin}&destination={destination}&departureDate={departureDate:yyyy-MM-dd}{returnDateParam}";
        
        var response = await _httpClient.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            return new List<FlightDto>();
        }

        return await response.Content.ReadFromJsonAsync<List<FlightDto>>() ?? new List<FlightDto>();
    }

    public async Task<FlightDto?> GetFlightByIdAsync(string flightId)
    {
        var response = await _httpClient.GetAsync($"/Flights/{flightId}");
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<FlightDto>();
    }

    public async Task<List<AirportDto>> GetAirportsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/Airports");
            
            if (!response.IsSuccessStatusCode)
            {
                return new List<AirportDto>();
            }

            return await response.Content.ReadFromJsonAsync<List<AirportDto>>() ?? new List<AirportDto>();
        }
        catch (Exception)
        {
            return new List<AirportDto>();
        }
    }
}
