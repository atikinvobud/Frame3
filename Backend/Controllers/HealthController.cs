using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("/health")]
public class HealthController : ControllerBase
{
    [HttpGet()]
    public IActionResult Get()
    {
        return Ok(new{
            Status ="Ok",
            Date = DateTime.UtcNow,
        });
    }
}