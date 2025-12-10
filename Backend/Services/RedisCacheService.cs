using StackExchange.Redis;
using System.Text.Json;
namespace Backend.Services;
public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase context;
    private readonly JsonSerializerOptions jsonOptions;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        context = redis.GetDatabase();

        jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetCachedAsync<T>(string key)
    {
        var cached = await context.StringGetAsync(key);

        if (cached.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>(cached!, jsonOptions);
    }

    public async Task SetCachedAsync<T>(string key, T data, int ttlSeconds)
    {
        var json = JsonSerializer.Serialize(data, jsonOptions);

        if (ttlSeconds > 0)
        {
            await context.StringSetAsync(
                key,
                json,
                TimeSpan.FromSeconds(ttlSeconds)
            );
        }
        else
        {
            await context.StringSetAsync(key, json);
        }
    }

}
