using AirportAPI.Models;

namespace AirportAPI.Services;

public interface IAirportService
{
    Task<List<Airport>> GetAll();
    Task<Airport?> GetByCode(string code);
    Task<Airport> AddAirport(Airport newAirport);
    Task<Airport> UpdateAirport(Airport newAirport);
    Task DeleteAirport(string code);
    Task<bool> AirportExists(string? code);
}
