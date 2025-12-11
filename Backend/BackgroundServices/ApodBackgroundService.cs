using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Backend.Models.Entities;
using Backend.Repository;

namespace Backend.Services;

public class ApodBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly string apodUrl;
    private readonly int intervalSeconds;

    public ApodBackgroundService(IHttpClientFactory httpClientFactory,IServiceScopeFactory scopeFactory,IOptions<ApiUrls> urls,IOptions<FetchTimes> times)
    {
        this.httpClientFactory = httpClientFactory;
        this.scopeFactory = scopeFactory;
        apodUrl = urls.Value.ApodUrl;
        intervalSeconds = times.Value.Apod;
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

                var response = await httpClient.GetStringAsync(apodUrl);

                await repository.AddAsync(new SpaceCache
                {
                    Source = "apod",
                    Payload = response,
                    FetchedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"APOD fetch error: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }
    }
}
