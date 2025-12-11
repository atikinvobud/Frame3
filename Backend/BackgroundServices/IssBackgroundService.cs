using Microsoft.Extensions.Hosting;
using Backend.Repository;
using Backend.Models.Entities;
using Microsoft.Extensions.Options;

namespace Backend.Services;

public class IssBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly string apiUrl;
    private readonly int fetchIntervalSeconds;

    public IssBackgroundService(IServiceScopeFactory scopeFactory,IHttpClientFactory httpClientFactory,IOptions<ApiUrls> urls,IOptions<FetchTimes> times)
    {
        this.scopeFactory = scopeFactory;
        this.httpClientFactory = httpClientFactory;
        apiUrl = urls.Value.IssUrl;
        fetchIntervalSeconds = times.Value.Iss;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IIssRepository>();
                var spacerepository = scope.ServiceProvider.GetRequiredService<ISpaceCacheRepository>();
                var httpClient = httpClientFactory.CreateClient();

                var response = await httpClient.GetStringAsync(apiUrl);

                if (!string.IsNullOrWhiteSpace(response))
                {
                    await repository.AddAsync(new IssFetchLog
                    {
                        SourceUrl = apiUrl,
                        Payload = response,
                        FetchedAt = DateTime.UtcNow
                    });
                    await spacerepository.AddAsync(new SpaceCache
                    {
                        Source = "iss",
                        Payload = response,
                        FetchedAt = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении ISS: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(fetchIntervalSeconds), stoppingToken);
        }
    }
}
