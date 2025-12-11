namespace Backend.Services;

public interface ITelemetryService
{
    Task GenerateAndStoreAsync(CancellationToken ct = default);
}
