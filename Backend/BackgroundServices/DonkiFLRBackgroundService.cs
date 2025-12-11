using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Backend.Models.Entities;
using Backend.Repository;

namespace Backend.Services;

public class DonkiFLRBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly string flrUrl;
    private readonly int intervalSeconds;

    public DonkiFLRBackgroundService(IHttpClientFactory httpClientFactory,IServiceScopeFactory scopeFactory,IOptions<ApiUrls> urls,IOptions<FetchTimes> times)
    {
        this.httpClientFactory = httpClientFactory;
        this.scopeFactory = scopeFactory;
        flrUrl = urls.Value.DonkiFLR;
        intervalSeconds = times.Value.Donki;
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

                var response = await httpClient.GetStringAsync(flrUrl);

                await repository.AddAsync(new SpaceCache
                {
                    Source = "flr",
                    Payload = response,
                    FetchedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FLR fetch error: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }
    }
}
