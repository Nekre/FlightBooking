using FlightBooking.Application.Features.Airports.Queries.GetAirports;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AirportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AirportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAirportsQuery());
        return Ok(result);
    }
}