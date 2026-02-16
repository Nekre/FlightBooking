namespace FlightBooking.Application.DTOs;

public class AirportDto
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    
    public AirportDto(string code, string name, string city)
    {
        Code = code;
        Name = name;
        City = city;
    }

}