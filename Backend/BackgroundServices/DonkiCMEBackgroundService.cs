using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Backend.Models.Entities;
using Backend.Repository;

namespace Backend.Services;

public class DonkiCMEBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly string cmeUrl;
    private readonly int intervalSeconds;

    public DonkiCMEBackgroundService(IHttpClientFactory httpClientFactory,IServiceScopeFactory scopeFactory,IOptions<ApiUrls> urls,IOptions<FetchTimes> times)
    {
        this.httpClientFactory = httpClientFactory;
        this.scopeFactory = scopeFactory;
        cmeUrl = urls.Value.DonkiCME;
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

                var response = await httpClient.GetStringAsync(cmeUrl);

                await repository.AddAsync(new SpaceCache
                {
                    Source = "cme",
                    Payload = response,
                    FetchedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CME fetch error: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }
    }
}
