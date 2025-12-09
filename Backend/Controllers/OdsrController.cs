using Backend.Repository;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("osdr")]
public class OsdrController : ControllerBase
{
    private readonly IOsdrService service;
    private readonly IOsdrRepository repository;

    public OsdrController(IOsdrService service, IOsdrRepository repository)
    {
        this.service = service;
        this.repository = repository;
    }

    [HttpGet("sync")]
    public async Task<IActionResult> Sync(CancellationToken ct)
    {
        var written = await service.FetchAndStoreAsync(ct);
        return Ok(new { message = "OSDR synced", written });
    }

    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var items = await repository.ListLatestAsync(limit, ct);
        return Ok(items);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await repository.GetAll(ct);
        return Ok(items);
    }
}
