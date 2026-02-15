namespace FlightBooking.Application.DTOs;


public class SearchResponseDto
{
    public List<FlightDto> OutboundFlights { get; set; } = new();
    public List<FlightDto> InboundFlights { get; set; } = new();
    public bool Success { get; set; } = true;
    public bool FromCache { get; set; }

}
