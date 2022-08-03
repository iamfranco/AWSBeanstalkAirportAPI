using AirportAPI.Models;
using Amazon.DynamoDBv2.DataModel;

namespace AirportAPI.Services;

public class AirportDynamoDBService : IAirportService
{
    private readonly IDynamoDBContext _context;

    public AirportDynamoDBService(IDynamoDBContext context)
    {
        _context = context;
    }

    public async Task<List<Airport>> GetAll()
    {
        List<Airport> airports = await _context.ScanAsync<Airport>(default).GetRemainingAsync();

        return airports;
    }

    public async Task<Airport?> GetByCode(string code)
    {
        Airport airport = await _context.LoadAsync<Airport>(code);

        return airport;
    }

    public async Task AddAirport(Airport newAirport)
    {
        await _context.SaveAsync(newAirport);
    }

    public async Task UpdateAirport(Airport newAirport)
    {
        await _context.SaveAsync(newAirport);
    }

    public async Task DeleteAirport(string code)
    {
        await _context.DeleteAsync(code);
    }

    public async Task<bool> AirportExists(string? code)
    {
        if (code is null)
            return true;

        Airport? airport = await GetByCode(code);

        return airport is not null;
    }
}
