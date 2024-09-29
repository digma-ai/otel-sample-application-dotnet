using Microsoft.AspNetCore.Mvc;

namespace Sample.MoneyTransfer.API.Controllers;


[ApiController]
[Route("[controller]/[action]")]
public class StatusController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await Task.Delay(5);
        return Ok();
    }
}