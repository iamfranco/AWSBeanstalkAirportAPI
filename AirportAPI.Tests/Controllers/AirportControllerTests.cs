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
        result.Result.Should().BeOfType(typeof(NotFoundObjectResult));
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

        // Act
        var actionResult = await _controller.AddAirport(newAirport);

        // Assert
        _mockAirportService.Verify(x => x.AddAirport(newAirport), Times.Never());
        actionResult.Result.Should().BeOfType(typeof(ConflictObjectResult));
    }

    [Test]
    public async Task UpdateAirport_Should_Update_Airport_And_Return_NoContent()
    {
        // Arrange
        Airport newAirport = new()
        {
            Code = "MAN",
            Name = "Manchester Super Airport",
            City = "Manchester"
        };

        _mockAirportService.Setup(x => x.AirportExists(newAirport.Code))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateAirport(newAirport);

        // Assert
        _mockAirportService.Verify(x => x.UpdateAirport(newAirport), Times.Once());
        result.Should().BeOfType(typeof(NoContentResult));
    }

    [Test]
    public async Task UpdateAirport_With_Airport_Not_Existing_Should_Return_NotFound()
    {
        // Arrange
        Airport newAirport = new()
        {
            Code = "ABC",
            Name = "ABC Airport",
            City = "Manchester"
        };

        _mockAirportService.Setup(x => x.AirportExists(newAirport.Code))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateAirport(newAirport);

        // Assert
        _mockAirportService.Verify(x => x.UpdateAirport(newAirport), Times.Never());
        result.Should().BeOfType(typeof(NotFoundObjectResult));
    }

    [Test]
    public async Task DeleteAirport_Should_Delete_Airport_And_Return_NoContent()
    {
        // Arrange
        _mockAirportService.Setup(x => x.AirportExists("MAN"))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteAirport("MAN");

        // Assert
        _mockAirportService.Verify(x => x.DeleteAirport("MAN"), Times.Once());
        result.Should().BeOfType(typeof(NoContentResult));
    }

    [Test]
    public async Task DeleteAirport_With_No_Airport_Matching_Code_Should_Return_NotFound()
    {
        // Arrange
        _mockAirportService.Setup(x => x.AirportExists("ABC"))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteAirport("ABC");

        // Assert
        _mockAirportService.Verify(x => x.DeleteAirport("ABC"), Times.Never());
        result.Should().BeOfType(typeof(NotFoundObjectResult));
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
