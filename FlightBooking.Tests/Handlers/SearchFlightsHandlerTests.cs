using FlightBooking.Application.DTOs;
using FlightBooking.Application.Features.Flights.Queries.Search;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Features.Flights.Queries.SearchFlights;
using Moq;
using FluentAssertions;

namespace FlightBooking.Tests.Handlers;

public class SearchFlightsHandlerTests
{
    private readonly Mock<IFlightService> _mockFlightService;
    private readonly SearchFlightsHandler _sut;

    public SearchFlightsHandlerTests()
    {
        _mockFlightService = new Mock<IFlightService>();
        _sut = new SearchFlightsHandler(_mockFlightService.Object);
    }

    [Fact]
    public async Task Handle_ShouldMapQueryToSearchRequestDto()
    {
        var query = new SearchFlightsQuery(
            Origin: "IST",
            Destination: "AMS",
            DepartureDate: new DateTime(2026, 2, 20),
            ReturnDate: new DateTime(2026, 2, 27)
        );

        var expectedResponse = new SearchResponseDto
        {
            OutboundFlights = new List<FlightDto>(),
            Success = true
        };

        _mockFlightService.Setup(x => x.SearchAsync(
            It.Is<SearchRequestDto>(r =>
                r.Origin == "IST" &&
                r.Destination == "AMS" &&
                r.DepartureDate.Date == new DateTime(2026, 2, 20).Date &&
                r.ReturnDate == new DateTime(2026, 2, 27)),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(expectedResponse);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _mockFlightService.Verify(x => x.SearchAsync(It.IsAny<SearchRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenReturnDateNull_ShouldPassNullToService()
    {
        var query = new SearchFlightsQuery(
            Origin: "IST",
            Destination: "AMS",
            DepartureDate: new DateTime(2026, 2, 20),
            ReturnDate: null
        );

        var expectedResponse = new SearchResponseDto
        {
            OutboundFlights = new List<FlightDto>(),
            Success = true
        };

        _mockFlightService.Setup(x => x.SearchAsync(
            It.Is<SearchRequestDto>(r => r.ReturnDate == null),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(expectedResponse);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        _mockFlightService.Verify(x => x.SearchAsync(
            It.Is<SearchRequestDto>(r => r.ReturnDate == null),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
