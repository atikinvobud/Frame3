using System.Text.Json;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backend.Controllers;

[ApiController]
[Route("apod")]
public class ApodController : ControllerBase
{
    private readonly IRedisCacheService redisCacheService;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly string apodUrl;
    public ApodController(IRedisCacheService redisCacheService, IHttpClientFactory httpClientFactory, IOptions<ApiUrls> urls)
    {
        this.redisCacheService = redisCacheService;
        this.httpClientFactory = httpClientFactory;
        apodUrl = urls.Value.ApodUrl;
    }

    [HttpGet("apod")]
    public async Task<IActionResult> GetApod()
    {
        string date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        string key = $"astronomy:apod:{date}";

        var cached = await redisCacheService.GetCachedAsync<ApodDto>(key);
        if (cached != null)
        {
            cached.Source = "Redis";
            return Ok(cached);
        }

        var httpClient = httpClientFactory.CreateClient();
        var json = await httpClient.GetStringAsync(apodUrl);
        var result = JsonSerializer.Deserialize<ApodDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result != null)
        {
            result.Source = "API";
            await redisCacheService.SetCachedAsync(key, result, ttlSeconds: 86400);
        }

        return Ok(result);
    }

}