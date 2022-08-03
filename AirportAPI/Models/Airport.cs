using Amazon.DynamoDBv2.DataModel;

namespace AirportAPI.Models;

[DynamoDBTable("airports")]
public class Airport
{
    [DynamoDBHashKey("code")]
    public string? Code { get; set; }

    [DynamoDBProperty("name")]
    public string? Name { get; set; }

    [DynamoDBProperty("city")]
    public string? City { get; set; }
}
