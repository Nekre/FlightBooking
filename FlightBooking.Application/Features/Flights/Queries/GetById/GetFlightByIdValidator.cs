using FlightBooking.Application.Features.Flights.Queries.GetById;
using FluentValidation;

namespace FlightBooking.Application.Features.Flights.Queries.GetById;

public class GetFlightByIdValidator : AbstractValidator<GetFlightByIdQuery>
{
    public GetFlightByIdValidator()
    {
        RuleFor(x => x.FlightId)
            .NotEmpty().WithMessage("Flight ID boş olamaz.")
            .MinimumLength(5).WithMessage("Geçerli bir Flight ID giriniz.");
    }
}
