using AirportAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirportAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AirportController : ControllerBase
{
    private IAirportService _airportService;

    public AirportController(IAirportService airportService)
    {
        _airportService = airportService;
    }
}
