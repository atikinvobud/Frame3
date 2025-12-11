using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Backend.Models.Entities;
using Backend.Repository;

namespace Backend.Services;

public class NeoBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly string neoUrl;
    private readonly int intervalSeconds;

    public NeoBackgroundService(IHttpClientFactory httpClientFactory,IServiceScopeFactory scopeFactory,IOptions<ApiUrls> urls,IOptions<FetchTimes> times)
    {
        this.httpClientFactory = httpClientFactory;
        this.scopeFactory = scopeFactory;
        neoUrl = urls.Value.NeoUrl;
        intervalSeconds = times.Value.Neo;
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

                var response = await httpClient.GetStringAsync(neoUrl);

                await repository.AddAsync(new SpaceCache
                {
                    Source = "neo",
                    Payload = response,
                    FetchedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neo fetch error: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }
    }
}
