using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("/iss")]
public class IssController : ControllerBase
{
    private readonly IIssApiService service;

    public IssController(IIssApiService service)
    {
        this.service = service;
    }

    [HttpGet("fetch")]
    public async Task<IActionResult> FetchCurrent()
    {
        var result = await service.FetchCurrentAsync();
        if (string.IsNullOrWhiteSpace(result))
            return StatusCode(500);
        return Ok(result);
    }

    [HttpGet("last")]
    public async Task<IActionResult> GetLast()
    {
        var last = await service.GetLastAsync();
        if (last == null)
            return NotFound();

        return Ok(last);
    }

    [HttpGet("trend")]
    public async Task<IActionResult> GetTrend()
    {
        var trend = await service.GetTrendAsync();
        return Ok(trend);
    }
}
