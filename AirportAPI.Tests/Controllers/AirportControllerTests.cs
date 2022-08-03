using AirportAPI.Controllers;
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
}
