using AirportAPI.Controllers;
using AirportAPI.Models;
using AirportAPI.Services;
using Moq;

namespace AirportAPI.Tests.Controllers;
internal class AirportControllerTests
{
    private Mock<IAirportService> _mockAirportService;
    private AirportController _controller;

    [SetUp]
    public void Setup()
    {
        _mockAirportService = new Mock<IAirportService>();
        _controller = new AirportController(_mockAirportService.Object);
    }

    [Test]
    public async Task GetAllAirports_Should_Return_List_Of_Airports()
    {
        // Arrange
        _mockAirportService.Setup(x => x.GetAll())
            .ReturnsAsync(GetAllAirports());

        // Act
        List<Airport> result = await _controller.GetAllAirports();

        // Assert
        result.Should().BeEquivalentTo(GetAllAirports());
    }

    private static List<Airport> GetAllAirports()
    {
        return new()
        {
            new()
            {
                Code = "MAN",
                Name = "Manchester Airport",
                City = "Manchester"
            },
            new()
            {
                Code = "LCY",
                Name = "London City Airport",
                City = "London"
            },
            new()
            {
                Code = "LHR",
                Name = "Heathrow Airport",
                City = "London"
            },
            new()
            {
                Code = "LGW",
                Name = "Gatwick Airport",
                City = "London"
            },
            new()
            {
                Code = "BHX",
                Name = "Birmingham Airport",
                City = "Birmingham"
            },
            new()
            {
                Code = "GLA",
                Name = "Glasgow Airport",
                City = "Glasgow"
            }
        };
    }
}
