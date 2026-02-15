using FlightBooking.Application.DTOs;
using MediatR;

namespace FlightBooking.Application.Features.Flights.Queries.Search;

public record SearchFlightsQuery(string? Origin, string? Destination, DateTime DepartureDate, DateTime? ReturnDate)
    : IRequest<SearchResponseDto>;