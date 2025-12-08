using Backend.Models.Entities;
using Backend.Repository;
using System.Net.Http.Json;
using System.Text.Json;

namespace Backend.Services;

public class IssApiService : IIssApiService
{
    private readonly HttpClient httpClient;
    private readonly IIssRepository repository;
    private const string IssApiUrl = "https://api.wheretheiss.at/v1/satellites/25544";

    public IssApiService(HttpClient httpClient, IIssRepository repository)
    {
        this.httpClient = httpClient;
        this.repository = repository;
    }

    public async Task<string?> FetchCurrentAsync()
    {
        var response = await httpClient.GetStringAsync(IssApiUrl);

        if (!string.IsNullOrWhiteSpace(response))
        {
            await repository.AddAsync(new IssFetchLog
            {
                SourceUrl = IssApiUrl,
                Payload = response,
                FetchedAt = DateTime.UtcNow
            });
        }

        return response;
    }

    public async Task<IssFetchLog?> GetLastAsync()
    {
        return await repository.GetLastAsync();
    }

    public async Task<object> GetTrendAsync()
    {
        var logs = await repository.GetLastNAsync(2);
        if (logs.Count < 2) return new { Movement = false };

        var from = JsonSerializer.Deserialize<IssPosition>(logs[1].Payload)!;
        var to = JsonSerializer.Deserialize<IssPosition>(logs[0].Payload)!;

        var deltaKm = GeoHelper.Haversine(from.Latitude, from.Longitude, to.Latitude, to.Longitude);
        var dtSec = (logs[0].FetchedAt - logs[1].FetchedAt).TotalSeconds;

        return new
        {
            Movement = deltaKm > 0.1,
            DeltaKm = deltaKm,
            DtSec = dtSec,
            VelocityKmh = to.Velocity,
            FromTime = logs[1].FetchedAt,
            ToTime = logs[0].FetchedAt,
            FromLat = from.Latitude,
            FromLon = from.Longitude,
            ToLat = to.Latitude,
            ToLon = to.Longitude
        };
    }
}
