namespace Mailroom.Pages;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var hostname = Environment.MachineName;

        var result = new
        {
            status = "ok",
            server = hostname
        };

        return Ok(result);
    }
}
