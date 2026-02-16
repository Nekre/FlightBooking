using FlightBooking.Application.DTOs;
using FlightBooking.Application.Interfaces;
using FlightBooking.Application.Services;
using FlightBooking.Application.Utilities;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace FlightBooking.Tests.Services;

public class FlightSearchServiceTests
{
    private readonly Mock<IFlightProvider> _mockProvider;
    private readonly Mock<ICacheService> _mockCache;
    private readonly Mock<ILogger<FlightSearchService>> _mockLogger;
    private readonly FlightSearchService _sut;

    public FlightSearchServiceTests()
    {
        _mockProvider = new Mock<IFlightProvider>();
        _mockCache = new Mock<ICacheService>();
        _mockLogger = new Mock<ILogger<FlightSearchService>>();
        _sut = new FlightSearchService(new[] { _mockProvider.Object }, _mockCache.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task SearchAsync_WhenCacheHit_ShouldReturnCachedResultWithFromCacheTrue()
    {
        var request = new SearchRequestDto
        {
            Origin = "IST",
            Destination = "AMS",
            DepartureDate = new DateTime(2026, 2, 20)
        };

        var cachedResponse = new SearchResponseDto
        {
            OutboundFlights = new List<FlightDto>
            {
                new FlightDto { FlightNumber = "TK123", Origin = "IST", Destination = "AMS" }
            },
            Success = true
        };

        _mockCache.Setup(x => x.GetAsync<SearchResponseDto>(
            It.Is<string>(k => k.Contains("IST") && k.Contains("AMS")),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(cachedResponse);

        var result = await _sut.SearchAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.FromCache.Should().BeTrue();
        result.OutboundFlights.Should().HaveCount(1);
        _mockProvider.Verify(x => x.GetFlightsAsync(It.IsAny<SearchRequestDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SearchAsync_WhenCacheMiss_ShouldCallProviderAndReturnResults()
    {
        var request = new SearchRequestDto
        {
            Origin = "IST",
            Destination = "AMS",
            DepartureDate = new DateTime(2026, 2, 20)
        };

        var providerFlights = new List<FlightDto>
        {
            new FlightDto
            {
                FlightNumber = "TK123",
                DepartureTime = new DateTime(2026, 2, 20, 14, 30, 0),
                ArrivalTime = new DateTime(2026, 2, 20, 17, 45, 0),
                Price = 450
            }
        };

        _mockCache.Setup(x => x.GetAsync<SearchResponseDto>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SearchResponseDto)null);

        _mockProvider.Setup(x => x.GetFlightsAsync(It.IsAny<SearchRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerFlights);

        _mockCache.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.SearchAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.FromCache.Should().BeFalse();
        result.OutboundFlights.Should().HaveCount(1);
        result.OutboundFlights[0].FlightNumber.Should().Be("TK123");
        result.OutboundFlights[0].Id.Should().Contain("TK123");
        result.InboundFlights.Should().BeEmpty();

        _mockProvider.Verify(x => x.GetFlightsAsync(It.IsAny<SearchRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WhenReturnDateProvided_ShouldSearchBothOutboundAndInbound()
    {
        var request = new SearchRequestDto
        {
            Origin = "IST",
            Destination = "AMS",
            DepartureDate = new DateTime(2026, 2, 20),
            ReturnDate = new DateTime(2026, 2, 27)
        };

        var outboundFlights = new List<FlightDto>
        {
            new FlightDto
            {
                FlightNumber = "TK123",
                DepartureTime = new DateTime(2026, 2, 20, 14, 30, 0),
                ArrivalTime = new DateTime(2026, 2, 20, 17, 45, 0)
            }
        };

        var inboundFlights = new List<FlightDto>
        {
            new FlightDto
            {
                FlightNumber = "TK456",
                DepartureTime = new DateTime(2026, 2, 27, 10, 0, 0),
                ArrivalTime = new DateTime(2026, 2, 27, 13, 15, 0)
            }
        };

        _mockCache.Setup(x => x.GetAsync<SearchResponseDto>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SearchResponseDto)null);

        _mockProvider.SetupSequence(x => x.GetFlightsAsync(It.IsAny<SearchRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outboundFlights)
            .ReturnsAsync(inboundFlights);

        _mockCache.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.SearchAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.OutboundFlights.Should().HaveCount(1);
        result.InboundFlights.Should().HaveCount(1);
        result.TotalFlights.Should().Be(2);
        
        _mockProvider.Verify(x => x.GetFlightsAsync(It.IsAny<SearchRequestDto>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task SearchAsync_WhenNoFlightsFound_ShouldReturnEmptyListWithSuccessTrue()
    {
        var request = new SearchRequestDto
        {
            Origin = "IST",
            Destination = "AMS",
            DepartureDate = new DateTime(2026, 2, 20)
        };

        _mockCache.Setup(x => x.GetAsync<SearchResponseDto>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SearchResponseDto)null);

        _mockProvider.Setup(x => x.GetFlightsAsync(It.IsAny<SearchRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FlightDto>());

        var result = await _sut.SearchAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.OutboundFlights.Should().BeEmpty();
        result.InboundFlights.Should().BeEmpty();
        result.TotalFlights.Should().Be(0);
    }

    [Fact]
    public async Task SearchAsync_ShouldEnrichFlightsWithId()
    {
        // Arrange
        var request = new SearchRequestDto
        {
            Origin = "IST",
            Destination = "AMS",
            DepartureDate = new DateTime(2026, 2, 20)
        };

        var providerFlights = new List<FlightDto>
        {
            new FlightDto
            {
                FlightNumber = "TK123",
                DepartureTime = new DateTime(2026, 2, 20, 14, 30, 0)
            }
        };

        _mockCache.Setup(x => x.GetAsync<SearchResponseDto>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SearchResponseDto)null);

        _mockProvider.Setup(x => x.GetFlightsAsync(It.IsAny<SearchRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerFlights);

        _mockCache.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        
        var result = await _sut.SearchAsync(request, CancellationToken.None);

        result.OutboundFlights[0].Id.Should().Be("TK123_202602201430");
    }
}
