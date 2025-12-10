namespace Backend.Services;
public class RateLimiterMiddleware
{
    private readonly RequestDelegate next;
    private readonly RedisRateLimiter limiter;

    public RateLimiterMiddleware(RequestDelegate next, RedisRateLimiter limiter)
    {
        this.next = next;
        this.limiter = limiter;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"ratelimit:{ip}";

        var allowed = await limiter.AllowRequestAsync(key);
        context.Response.Headers["X-RateLimit-Limit"] = limiter.maxRequests.ToString();
        
        if (!allowed)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Слишком много запросов, попробуйте позже.");
            return;
        }

        await next(context);
    }
}