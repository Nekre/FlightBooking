using FlightBooking.Application.DTOs;
using MediatR;

namespace FlightBooking.Application.Features.Flights.Queries.GetById;

public record GetFlightByIdQuery(string FlightId) : IRequest<FlightDto?>;
