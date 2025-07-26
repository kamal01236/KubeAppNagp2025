using Microsoft.AspNetCore.Mvc;

[ApiController]
 [Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet("ready")]
    public IActionResult Readiness() => Ok("Ready");

    [HttpGet("live")]
    public IActionResult Liveness() => Ok("Alive");
}