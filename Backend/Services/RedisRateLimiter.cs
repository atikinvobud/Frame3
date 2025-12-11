using StackExchange.Redis;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
namespace Backend.Services;
public class RedisRateLimiter
{
    private readonly IDatabase context;
    public readonly int maxRequests;
    private readonly TimeSpan window;

    public RedisRateLimiter(IConnectionMultiplexer redis, int maxRequests = 10, TimeSpan? window = null)
    {
        context = redis.GetDatabase();
        this.maxRequests = maxRequests;
        this.window = window ?? TimeSpan.FromMinutes(1);
    }

    public async Task<bool> AllowRequestAsync(string key)
    {
        var count = await context.StringIncrementAsync(key);

        if (count == 1)
        {
            await context.KeyExpireAsync(key, window);
        }

        return count <= maxRequests;
    }
}