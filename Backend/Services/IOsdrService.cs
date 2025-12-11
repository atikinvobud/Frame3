namespace Backend.Services;

public interface IOsdrService
{
    Task<int> FetchAndStoreAsync(CancellationToken ct = default);
}
