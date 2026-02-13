namespace FlightBooking.Application.DTOs;

public class FlightDto
{
    public string FlightNumber { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Price { get; set; }
    
    public TimeSpan Duration => ArrivalTime - DepartureTime;
}