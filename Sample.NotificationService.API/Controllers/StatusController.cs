using Microsoft.AspNetCore.Mvc;

namespace Sample.NotificationService.API.Controllers;


[ApiController]
[Route("[controller]/[action]")]
public class StatusController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await Task.Delay(100);
        return Ok();
    }
}