using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Backend.Models.Entities;
using Backend.Repository;

namespace Backend.Services;

public class SpaceXBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly string spaceXUrl;
    private readonly int intervalSeconds;

    public SpaceXBackgroundService(IHttpClientFactory httpClientFactory,IServiceScopeFactory scopeFactory,IOptions<ApiUrls> urls,IOptions<FetchTimes> times)
    {
        this.httpClientFactory = httpClientFactory;
        this.scopeFactory = scopeFactory;
        spaceXUrl = urls.Value.SpaceXUrl;
        intervalSeconds = times.Value.Spacex;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = httpClientFactory.CreateClient();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ISpaceCacheRepository>();
                var httpClient = httpClientFactory.CreateClient();

                var response = await httpClient.GetStringAsync(spaceXUrl);

                await repository.AddAsync(new SpaceCache
                {
                    Source = "spacex",
                    Payload = response,
                    FetchedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SpaceX fetch error: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }
    }
}
