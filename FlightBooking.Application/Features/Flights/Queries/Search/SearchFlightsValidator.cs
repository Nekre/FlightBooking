using FlightBooking.Application.Features.Flights.Queries.Search;
using FluentValidation;

namespace FlightBooking.Application.Features.Flights.Queries.SearchFlights;

public class SearchFlightsValidator : AbstractValidator<SearchFlightsQuery>
{
    public SearchFlightsValidator()
    {
        RuleFor(x => x.Origin)
            .NotEmpty().WithMessage("Kalkış noktası boş olamaz.")
            .Length(3).WithMessage("Havaalanı kodu 3 karakter olmalıdır.");

        RuleFor(x => x.Destination)
            .NotEmpty().WithMessage("Varış noktası boş olamaz.")
            .Length(3).WithMessage("Havaalanı kodu 3 karakter olmalıdır.")
            .NotEqual(x => x.Origin).WithMessage("Kalkış ve varış noktası aynı olamaz.");

        RuleFor(x => x.DepartureDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Geçmiş bir tarihe uçuş arayamazsınız.");

        RuleFor(x => x.ReturnDate)
            .GreaterThanOrEqualTo(x => x.DepartureDate)
            .WithMessage("Dönüş tarihi, gidiş tarihinden önce olamaz.")
            .When(x => x.ReturnDate.HasValue);
    }
}