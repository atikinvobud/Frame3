using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Services;

public class TelemetryBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly int intervalSeconds;

    public TelemetryBackgroundService(IServiceScopeFactory scopeFactory, IOptions<FetchTimes> times)
    {
        this.scopeFactory = scopeFactory;
        intervalSeconds = times.Value.Telemetry;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {   
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<ITelemetryService>();
                await service.GenerateAndStoreAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Telemetry generation error: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }
    }
}
