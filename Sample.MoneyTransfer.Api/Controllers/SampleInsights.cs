using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace Sample.MoneyTransfer.API.Controllers;

class SampleInsightsService
{
    private static readonly ActivitySource Activity = new(nameof(SampleInsightsService));

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

    public void ThrowArgumentException()
    {
        using var activity = Activity.StartActivity("Rethrow2");
        {
            throw new ArgumentException("empty argument2");
        }

    }
    
    public void HandledException()
    {
        using var activity = Activity.StartActivity("HandledException");
        {
            try
            {
                throw new ArgumentException("empty argument");
            }
            catch (Exception ex)
            {
                activity.RecordException(ex);
            }
            
        }

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
    [Route("Rethrow2")]
    public async Task Rethrow2()
    {
        using var activity = Activity.StartActivity("Rethrow2");
        await Task.Delay(TimeSpan.FromMilliseconds(1));
        try
        {
            _service.ThrowArgumentException();
        }
        catch(Exception ex)
        {
            activity.RecordException(ex);
            throw new ArgumentException("empty argument", ex);
        }
        
    }

    [HttpGet]
    [Route("Rethrow1")]
    public async Task Rethrow1()
    {
        using var activity = Activity.StartActivity("Rethrow1");
        await Task.Delay(TimeSpan.FromMilliseconds(1));
        try
        {
            _service.ThrowArgumentException();
        }
        catch(Exception ex)
        {
            throw new ArgumentException("empty argument", ex);
        }
    }
    
    [HttpGet]
    [Route("Handled")]
    public async Task Handled()
    {
        using var activity = Activity.StartActivity("Handled");
        await Task.Delay(TimeSpan.FromMilliseconds(1));
        _service.HandledException();
        throw new ArgumentException("empty argument");
    }
    
    [HttpGet]
    [Route("ErrorSource")]
    public async Task ErrorSource()
    {
        using var activity = Activity.StartActivity("ErrorSource");
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
        using var activity = Activity.StartActivity("Error");
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