namespace FlightBooking.Application.DTOs;

public class SearchRequestDto
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    
    public SearchRequestDto ShallowCopy()
    {
        return new SearchRequestDto
        {
            Origin = this.Origin,
            Destination = this.Destination,
            DepartureDate = this.DepartureDate,
            ReturnDate = this.ReturnDate
        };
    }


    public SearchRequestDto? CreateInboundRequest()
    {
        if (!this.ReturnDate.HasValue)
            return null;

        return new SearchRequestDto
        {
            Origin = this.Destination,
            Destination = this.Origin,
            DepartureDate = this.ReturnDate.Value, 
            ReturnDate = null
        };
    }
}