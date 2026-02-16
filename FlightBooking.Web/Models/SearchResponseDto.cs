namespace FlightBooking.Web.Models;
public class SearchResponseDto
{
    public List<FlightDto> OutboundFlights { get; set; } = new();
    public List<FlightDto> InboundFlights { get; set; } = new();
    public int TotalFlights => OutboundFlights.Count + InboundFlights.Count;
    public bool Success { get; set; } = true;
    public bool FromCache { get; set; }
}
