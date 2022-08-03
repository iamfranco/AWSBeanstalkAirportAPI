using AirportAPI.Models;

namespace AirportAPI.Services;

public interface IAirportService
{
    Task<List<Airport>> GetAll();
}
