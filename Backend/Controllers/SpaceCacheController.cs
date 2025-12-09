using Backend.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("space")]
public class SpaceCacheController : ControllerBase
{
    private readonly ISpaceCacheService service;

    public SpaceCacheController(ISpaceCacheService service)
    {
        this.service = service;
    }

    [HttpGet("{src}/latest")]
    public async Task<IActionResult> GetLatest(string src)
    {
        var item = await service.GetLatestAsync(src);

        if (item == null)
            return NotFound(new { message = $"No data for source '{src}'" });

        return Ok(item);
    }

    
}
