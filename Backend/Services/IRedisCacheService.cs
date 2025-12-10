namespace Backend.Services;
public interface IRedisCacheService
{
    Task<T?> GetCachedAsync<T>(string key);
    Task SetCachedAsync<T>(string key, T data, int ttlSeconds);
}
