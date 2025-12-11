using Backend.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Backend.Services;

public class OsdrBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly int intervalSeconds;

    public OsdrBackgroundService(IServiceScopeFactory scopeFactory,IOptions<FetchTimes> times)
    {
        this.scopeFactory = scopeFactory;
        intervalSeconds = times.Value.Osdr; 
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IOsdrService>();
                var written = await service.FetchAndStoreAsync(stoppingToken);
                Console.WriteLine($"[OSDR] Updated {written} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OSDR] Error: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }
    }
}
