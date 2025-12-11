using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("astro")]
public class AstroController : ControllerBase
{
    private readonly AstroHelper helper;

    public AstroController(AstroHelper helper)
    {
        this.helper = helper;
    }

    [HttpGet("events")]
public async Task<IActionResult> GetEvents(
    [FromQuery] double lat = 55.7558,
    [FromQuery] double lon = 37.6176,
    [FromQuery] int days = 7)
{
    string[] bodies = { "sun", "moon"};
    var allEvents = new List<object>();

    foreach (var body in bodies)
    {
        var data = await helper.GetEventsAsync(body, lat, lon, days);
        allEvents.Add(new { body, data });
    }

    return Ok(allEvents);
}

}

