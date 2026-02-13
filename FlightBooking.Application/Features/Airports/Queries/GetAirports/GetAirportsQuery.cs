using MediatR;
using FlightBooking.Application.DTOs;

namespace FlightBooking.Application.Features.Airports.Queries.GetAirports;

public record GetAirportsQuery : IRequest<List<AirportDto>>;