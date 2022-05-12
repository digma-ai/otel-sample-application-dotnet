using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;

namespace Sample.MoneyTransfer.API.Controllers;

class SampleInsightsService
{
    private void Connect()
    {
        throw new ConnectionAbortedException("aborting connection");
    }
    public void DoSomething()
    {
        Connect();
    }
    public void DoSomethingElse()
    {
        Connect();
    }
}
[ApiController]
[Route("[controller]")]
public class SampleInsightsController  : ControllerBase
{
    private static readonly ActivitySource Activity = new(nameof(SampleInsightsController));
    private static readonly Random Random = new();
    private readonly SampleInsightsService _service;
    public SampleInsightsController()
    {
        _service = new SampleInsightsService();
    }
    
    [HttpGet]
    [Route("ErrorSource")]
    public async Task ErrorSource()
    {
        using var activity = Activity.StartActivity(nameof(ErrorSource));
        await Task.Delay(TimeSpan.FromMilliseconds(1));
        if (Random.Next(1, 10) % 2 == 0)
        {
            _service.DoSomething();
        }
        else
        {
             _service.DoSomethingElse();
        }
    }
    
    [HttpGet]
    [Route("Error")]
    public async Task Error()
    {
        using var activity = Activity.StartActivity(nameof(Error));
        await Task.Delay(TimeSpan.FromMilliseconds(1));
        if (Random.Next(1, 10) % 2 == 0)
        {
            throw new InvalidOperationException("operation is not valid");
        }

        throw new ValidationException("random validation error");
    }
    
    [HttpGet]
    [Route("SlowEndpoint")]
    public async Task SlowEndpoint([FromQuery]int extraLatency)
    {
        await Task.Delay(extraLatency);
    }
    
    [HttpGet]
    [Route("SpanBottleneck")]
    public async Task SpanBottleneck()
    {
        using var activity1 = Activity.StartActivity("SpanBottleneck 1");
        await Task.Delay(TimeSpan.FromMilliseconds(200));

        using var activity2 = Activity.StartActivity("SpanBottleneck 2");
        await Task.Delay(TimeSpan.FromMilliseconds(100));
    }
    
    /*
     *1       1  
     * *3     10 mid
     * *1     20 calls
     */
    [HttpGet]
    [Route("LowUsage")]
    public async Task LowUsage()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(5));
    }
    
    [HttpGet]
    [Route("NormalUsage")]
    public async Task NormalUsage()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(5));
    }
    
    [HttpGet]
    [Route("HighUsage")]
    public async Task HighUsage()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(5));
    }
    
}