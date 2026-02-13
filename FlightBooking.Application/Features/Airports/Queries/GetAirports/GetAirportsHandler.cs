using MediatR;
using FlightBooking.Application.DTOs;
using FlightBooking.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Application.Features.Airports.Queries.GetAirports;

public class GetAirportsHandler : IRequestHandler<GetAirportsQuery, List<AirportDto>>
{
    private readonly IFlightDbContext _context;

    public GetAirportsHandler(IFlightDbContext context)
    {
        _context = context;
    }

    public async Task<List<AirportDto>> Handle(GetAirportsQuery request, CancellationToken cancellationToken)
    {
        var airports = await _context.Airports
            .Select(a => new AirportDto
            {
                Code = a.Code,
                Name = a.Name,
                City = a.City
            })
            .ToListAsync(cancellationToken);

        return airports;
    }
}