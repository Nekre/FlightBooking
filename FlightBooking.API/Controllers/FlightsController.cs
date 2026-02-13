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
        var results = await _mediator.Send(query);
        return results.Any() ? Ok(results) : NotFound();
    }
}