using AirportAPI.Controllers;
using AirportAPI.Models;
using AirportAPI.Services;
using Microsoft.AspNetCore.Mvc;
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
    public async Task GetAllAirports_Should_Return_Status_Ok_With_List_Of_Airports()
    {
        // Arrange
        _mockAirportService.Setup(x => x.GetAll())
            .ReturnsAsync(GetAllAirports());

        // Act
        var result = await _controller.GetAllAirports();

        // Assert
        result.Should().BeOfType(typeof(ActionResult<List<Airport>>));
        result.Value.Should().BeEquivalentTo(GetAllAirports());
    }

    [Test]
    public async Task GetAirportByCode_Should_Return_Correct_Airport()
    {
        // Arrange
        Airport expectedResult = GetAllAirports().Single(x => x.Code == "LHR");

        _mockAirportService.Setup(x => x.GetByCode("LHR"))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetAirportByCode("LHR");

        // Assert
        result.Should().BeOfType(typeof(ActionResult<Airport>));
        result.Value.Should().BeEquivalentTo(expectedResult);
    }

    [Test]
    public async Task GetAirportByCode_With_No_Match_Code_Should_Return_NotFound()
    {
        // Arrange
        Airport? expectedResult = null;

        _mockAirportService.Setup(x => x.GetByCode("ABC"))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetAirportByCode("ABC");

        // Assert
        result.Result.Should().BeOfType(typeof(NotFoundResult));
    }

    [Test]
    public async Task AddAirport_Should_Add_Airport_And_Return_No_Content()
    {
        // Arrange
        Airport newAirport = new()
        {
            Code = "JFK",
            Name = "John F. Kennedy International Airport",
            City = "New York"
        };

        _mockAirportService.Setup(x => x.AirportExists(newAirport.Code))
            .ReturnsAsync(false);

        _mockAirportService.Setup(x => x.AddAirport(newAirport))
            .ReturnsAsync(newAirport);

        // Act
        var actionResult = await _controller.AddAirport(newAirport);

        // Assert
        _mockAirportService.Verify(x => x.AddAirport(newAirport), Times.Once());
        var result = actionResult.Result as CreatedAtActionResult;
        result.Should().NotBeNull();
        result!.Value.Should().BeEquivalentTo(newAirport);
    }

    [Test]
    public async Task AddAirport_With_Airport_Already_Exist_Should_Return_Conflict()
    {
        // Arrange
        Airport newAirport = new()
        {
            Code = "MAN",
            Name = "Manchester Airport",
            City = "Manchester"
        };

        _mockAirportService.Setup(x => x.AirportExists(newAirport.Code))
            .ReturnsAsync(true);

        _mockAirportService.Setup(x => x.AddAirport(newAirport))
            .ReturnsAsync(newAirport);

        // Act
        var actionResult = await _controller.AddAirport(newAirport);

        // Assert
        _mockAirportService.Verify(x => x.AirportExists(newAirport.Code), Times.Once());
        actionResult.Result.Should().BeOfType(typeof(ConflictObjectResult));
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
