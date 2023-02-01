using Microsoft.AspNetCore.Mvc;

namespace Sample.MoneyTransfer.Gateway.Controllers;

[ApiController]
[Route("[controller]")]
public class GatewayController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<string>> GetAccount(long id)
    {
        HttpClient client = new HttpClient();  
        //var result = await client.GetAsync($"http://localhost:7151/Account/{id}?all=true");
        var result = await client.GetAsync($"http://www.example.com");

        return string.Empty;
    }
   
}