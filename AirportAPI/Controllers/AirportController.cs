using AirportAPI.Models;
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

    [HttpGet]
    public async Task<ActionResult<List<Airport>>> GetAllAirports()
    {
        List<Airport> airports = await _airportService.GetAll();

        return airports;
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<Airport>> GetAirportByCode(string code)
    {
        Airport? airport = await _airportService.GetByCode(code);

        if (airport is null)
            return NotFound();

        return airport;
    }

    [HttpPost]
    public async Task<ActionResult<Airport>> AddAirport(Airport newAirport)
    {
        if (await _airportService.AirportExists(newAirport.Code))
            return Conflict(new { message = $"Airport with code {newAirport.Code} already exists" });

        await _airportService.AddAirport(newAirport);

        return CreatedAtAction(
            nameof(GetAirportByCode),
            new { code = newAirport.Code },
            newAirport);
    }
}
