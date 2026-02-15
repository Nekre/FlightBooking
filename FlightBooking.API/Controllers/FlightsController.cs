using FlightBooking.Application.Features.Flights.Queries.GetById;
using FlightBooking.Application.Features.Flights.Queries.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class FlightsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FlightsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchFlightsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{flightId}")]
    public async Task<IActionResult> GetById(string flightId)
    {
        var query = new GetFlightByIdQuery(flightId);
        var result = await _mediator.Send(query);
        return result != null ? Ok(result) : NotFound(new { message = "Flight not found in cache" });
    }
}
