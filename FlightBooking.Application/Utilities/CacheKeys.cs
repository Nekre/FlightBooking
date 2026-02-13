using FlightBooking.Application.DTOs;

namespace FlightBooking.Application.Utilities;

public static class CacheKeys
{
    public static string SearchKey(SearchRequestDto request)
    {
        var returnDatePart = request.ReturnDate.HasValue
            ? $"_{request.ReturnDate.Value:yyyyMMdd}"
            : string.Empty;

        return $"Search_{request.Origin}_{request.Destination}_{request.DepartureDate:yyyyMMdd}{returnDatePart}";
    }

    public static string FlightKey(string flightId) => $"Flight_{flightId}";
}
