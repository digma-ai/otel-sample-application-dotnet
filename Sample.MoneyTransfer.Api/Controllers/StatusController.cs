using Microsoft.AspNetCore.Mvc;
using Sample.MoneyTransfer.API.Domain.Services;

namespace Sample.MoneyTransfer.API.Controllers;


[ApiController]
[Route("[controller]")]
public class StatusController : ControllerBase
{
    private readonly IStatusService _statusService;

    public StatusController(IStatusService statusService)
    {
        _statusService = statusService;
    }
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await Task.Delay(5);
        return Ok();
    }
    
    [HttpGet("error")]
    public async Task<IActionResult> Error()
    {
        await _statusService.Check();
        return Ok();
    }
}