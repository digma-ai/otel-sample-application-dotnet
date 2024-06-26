using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using Sample.MoneyTransfer.API.Consumer;
using Sample.MoneyTransfer.API.Domain.Services;
using Sample.MoneyTransfer.API.Utils;

namespace Sample.MoneyTransfer.API.Controllers;

class SampleInsightsService
{
    private static readonly ActivitySource Activity = new(nameof(SampleInsightsService));

    public SampleInsightsService()
    {
    }

    private void Connect(string connectionName)
    {
        throw new ConnectionAbortedException($"aborting connection named {connectionName}");
    }

    private void Connect()
    {
        Connect("basic");
    }

    // method DoSomething has Overloading implementations
    public void DoSomething()
    {
        Connect();
    }

    public void DoSomething(int int1)
    {
        Connect($"basic{int1}");
    }

    public void DoSomething(string str1)
    {
        Connect(str1);
    }

    public void DoSomething(string str1, out bool bool1, IList<string[][,,]> listOfArray)
    {
        bool1 = true;
        Connect(str1);
    }

    public void DoSomething(List<dynamic>[] arrayOfList, ref int int1)
    {
        Connect();
    }

    public void DoSomething(ref long[] longsArr1, IEnumerable<string> enumerable1, Func<int, string> func1)
    {
        var str = func1.Invoke(7);
        Connect(str);
    }

    public void DoSomething(IDictionary<string, IList<Func<int[]>>> dict1, double[][][] doublesJaggedArr1)
    {
        Connect();
    }

    public void DoSomething(ICollection<object> objectsColl1, long[,,][,,,,][,][,,,] longsMultidimensionalArr1)
    {
        Connect();
    }

    public void DoSomething(Func<int, int, string> func1, float[][,,,][][,,] floatsMixJaggedAndMultidimensionalArr1)
    {
        var str = func1.Invoke(7, 8);
        Connect(str);
    }

    public void DoSomethingElse()
    {
        Connect("else");
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
public class SampleInsightsController : ControllerBase
{
    private static readonly ActivitySource Activity = new(nameof(SampleInsightsController));
    private static readonly Random Random = new(Math.Abs((int)DateTime.Now.Ticks));
    private readonly SampleInsightsService _service;
    private readonly IQueryOptimizationService _queryOptimizationService;
    private readonly IMessagePublisher _messagePublisher;

    public SampleInsightsController(IQueryOptimizationService queryOptimizationService, IMessagePublisher messagePublisher)
    {
        _service = new SampleInsightsService();
        _queryOptimizationService = queryOptimizationService;
        _messagePublisher = messagePublisher;
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
        catch (Exception ex)
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
        catch (Exception ex)
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
    [Route("ErrorOnRequest")]
    public async Task ErrorOnRequest(bool throwError = false)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(1));
        if (throwError)
            throw new ArgumentException("empty argument");
    }

    [HttpGet]
    [Route("ErrorSource")]
    public async Task ErrorSource()
    {
        using var activity = Activity.StartActivity("ErrorSource");
        await Task.Delay(TimeSpan.FromMilliseconds(1));

        var randVal = Random.Next(1, 12);
        switch (randVal)
        {
            case 1:
                _service.DoSomethingElse();
                break;
            case 2:
                _service.DoSomething();
                break;
            case 3:
                _service.DoSomething(3);
                break;
            case 4:
                _service.DoSomething("lets go");
                break;
            case 5:
                _service.DoSomething("yes", out bool bool1, new List<string[][,,]>());
                break;
            case 6:
                int int1 = 5;
                _service.DoSomething(new[] { new List<dynamic>() }, ref int1);
                break;
            case 7:
                var longsArr = new long[] { };
                _service.DoSomething(ref longsArr, new string[] { }, x => $"val={x}");
                break;
            case 8:
                _service.DoSomething(new Dictionary<string, IList<Func<int[]>>>(), new double[][][] { });
                break;
            case 9:
                _service.DoSomething(new object[] { }, new long[,,][,,,,][,][,,,] { });
                break;
            case 10:
                _service.DoSomething((x, y) => $"sum={x + y}", new float[][,,,][][,,] { });
                break;
            default:
                _service.DoSomething();
                break;
        }
    }

    [HttpGet]
    [Route("UnverifiedChange/{milisec}")]
    public async Task UnverifiedChange(int milisec)
    {
        await DelayAsync(TimeSpan.FromMilliseconds(milisec));
    }

    [HttpGet]
    [Route("PureDelay/{milisec}")]
    public async Task PureDelay(int milisec)
    {
        await Task.Delay(milisec);
    }
    
    [HttpGet]
    [Route("PureDelay2/{milisec}")]
    public async Task PureDelay2(int milisec)
    {
        await Task.Delay(milisec);
    }
    
    [HttpGet]
    [Route("PureDelay4/{milisec}")]
    public async Task PureDelay4(int milisec)
    {
        await Task.Delay(milisec);
    }
    
    [HttpGet]
    [Route("PureDelay5/{milisec}")]
    public async Task PureDelay5(int milisec)
    {
        await Task.Delay(milisec);
    }
    
    [HttpGet]
    [Route("PureDelay6/{milisec}")]
    public async Task PureDelay6(int milisec)
    {
        await Task.Delay(milisec);
    }
    
    [HttpGet]
    [Route("PureDelay7/{milisec}")]
    public async Task PureDelay7(int milisec)
    {
        await Task.Delay(milisec);
    }
    
    [HttpGet]
    [Route("PureDelay8/{milisec}")]
    public async Task PureDelay8(int milisec)
    {
        await Task.Delay(milisec);
    }
    
     


    [HttpGet]
    [Route("Nesting/{milisec}")]
    public async Task Nesting(int milisec)
    {
        await Child1(milisec);
    }

    private async Task Child1(int milisec)
    {
        using var activity = Activity.StartActivity();
        await Child2(milisec);
    }

    private async Task Child2(int milisec)
    {
        using var activity = Activity.StartActivity();
        await Task.Delay(milisec * 2);
    }

    [HttpGet]
    [Route("Delay/{milisec}")]
    public async Task Delay(int milisec)
    {
        await DelayAsync(TimeSpan.FromMilliseconds(milisec));
    }

    private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

    [HttpGet]
    [Route("lock/{milisec}")]
    public async Task Lock(double milisec = 10)
    {
        using (Activity.StartActivity("Connecting"))
        {
            await DelayAsync(TimeSpan.FromSeconds(2));
        }

        var dist = new MathNet.Numerics.Distributions.Normal(milisec, milisec / 10);
        await WaitForLock(dist);
    }

    [HttpGet]
    [Route("lock1/{milisec}")]
    public async Task Lock1(double milisec = 10)
    {
        using (Activity.StartActivity("Connecting1"))
        {
            await DelayAsync(TimeSpan.FromSeconds(2));
        }

        var dist = new MathNet.Numerics.Distributions.Normal(milisec, milisec / 10);
        await WaitForLock(dist);
    }

    private async Task Internal()
    {
        using var activity1 = Activity.StartActivity("Internal", ActivityKind.Internal);
        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }

    private async Task Service1()
    {
        using var activity1 = Activity.StartActivity("Service1", ActivityKind.Server);
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        await Internal();
    }


    private async Task Service2()
    {
        using var activity2 = Activity.StartActivity("Service2", ActivityKind.Client);
        await Service1();
    }

    private async Task Service3()
    {
        using var activity2 = Activity.StartActivity("Service3", ActivityKind.Client);
        await Service1();
    }


    [HttpGet]
    [Route("ServiceEndpoint")]
    public async Task ServiceEndpoint()
    {
        await Service2();
        await Service3();
    }


    [HttpGet]
    [Route("lock2/{milisec}")]
    public async Task Lock2(double milisec = 10)
    {
        using (Activity.StartActivity("Connecting2"))
        {
            await DelayAsync(TimeSpan.FromSeconds(2));
        }

        var dist = new MathNet.Numerics.Distributions.Normal(milisec, milisec / 10);
        await WaitForLock(dist);
    }

    [HttpGet]
    [Route("lock3/{milisec}")]
    public async Task Lock3(double milisec = 10)
    {
        using (Activity.StartActivity("Connecting3"))
        {
            await DelayAsync(TimeSpan.FromSeconds(2));
        }

        var dist = new MathNet.Numerics.Distributions.Normal(milisec, milisec / 10);
        await WaitForLock(dist);
    }

    [HttpGet]
    [Route("lock4/{milisec}")]
    public async Task Lock4(double milisec = 10)
    {
        using (Activity.StartActivity("Connecting4"))
        {
            await DelayAsync(TimeSpan.FromSeconds(1));
        }

        var dist = new MathNet.Numerics.Distributions.Normal(milisec, milisec / 10);
        await WaitForLock(dist);
    }

    [HttpGet]
    [Route("lock5/{milisec}")]
    public async Task Lock5(double milisec = 10)
    {
        using (Activity.StartActivity("Connecting5"))
        {
            await DelayAsync(TimeSpan.FromSeconds(1));
        }

        var dist = new MathNet.Numerics.Distributions.Normal(milisec, milisec / 10);
        await WaitForLock(dist);
    }

    [HttpGet]
    [Route("lock6/{milisec}")]
    public async Task Lock6(double milisec = 10)
    {
        using (Activity.StartActivity("Connecting6"))
        {
            await DelayAsync(TimeSpan.FromSeconds(1));
        }

        var dist = new MathNet.Numerics.Distributions.Normal(milisec, milisec / 10);
        await WaitForLock(dist);
    }

    private static async Task WaitForLock(Normal dist)
    {
        using var activity = Activity.StartActivity();
        await _semaphoreSlim.WaitAsync();
        try
        {
            await DelayAsync(TimeSpan.FromMilliseconds(dist.Sample()));
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    private static async Task DelayAsync(TimeSpan timeSpan)
    {
        using var activity = Activity.StartActivity();
        await Task.Delay(timeSpan);
    }


    [HttpGet]
    [Route(nameof(BottleNeckTest))]
    public void BottleNeckTest()
    {
        DbQueryUsersBottleNeck();
        DbQueryAccountsBottleNeck();
    }
    
    [HttpGet]
    [Route(nameof(NPlusOneOnlyEndpoint))]
    public void NPlusOneOnlyEndpoint()
    {
        Enumerable.Range(0, 10).Foreach(_ => DbQueryAccounts());
        Enumerable.Range(0, 10).Foreach(_ => DbQueryAccounts2());
    }
    
    [HttpGet]
    [Route(nameof(DbQuery10ms1))]
    public void DbQuery10ms1()
    {
        DbQueryAccounts10ms1();
    }
    
    [HttpGet]
    [Route(nameof(DbQuery10ms2))]
    public void DbQuery10ms2()
    {
        DbQueryAccounts10ms2();
    }
    
    [HttpGet]
    [Route(nameof(DbQuery10ms3))]
    public void DbQuery10ms3()
    {
        DbQueryAccounts10ms3();
    }
    
    /*
     * 
     */
    
    [HttpGet]
    [Route(nameof(NPlusOneSingleSpan))]
    public void NPlusOneSingleSpan() // check if this entrypoint has span with RootEndpointName == NPlusOneSingleSpan
    {
        using var activity1 = Activity.StartActivity("NewInternalSpan1"); // check if this span has any db...
        
        Enumerable.Range(0, 10).Foreach(_ => DbQueryAccounts()); // RootEndpointName == NPlusOneSingleSpan
                                                                 // ParentSpanObjectId == NewInternalSpan1
        Enumerable.Range(0, 10).Foreach(_ => DbQueryAccounts2()); //RootEndpointName == NPlusOneSingleSpan
    }
    
    [HttpGet]
    [Route(nameof(NPlusOneMultipleSpan))]
    public void NPlusOneMultipleSpan()
    {
        using var activity1 = Activity.StartActivity("NewInternalSpan1");

        Enumerable.Range(0, 10).Foreach(_ => DbQueryUsers());
        Enumerable.Range(0, 10).Foreach(_ => DbQueryAccounts());
        Enumerable.Range(0, 80).Foreach(_ => DbQueryRoles());
        Enumerable.Range(0, 70).Foreach(_ => DbQueryGroups());
        
        using var activity2 = Activity.StartActivity("NewInternalSpan2");

        Enumerable.Range(0, 30).Foreach(_ => DbQueryUsers());
        Enumerable.Range(0, 30).Foreach(_ => DbQueryAccounts());
        Enumerable.Range(0, 80).Foreach(_ => DbQueryRoles());
        Enumerable.Range(0, 70).Foreach(_ => DbQueryGroups());
    }


    [HttpGet]
    [Route(nameof(NPlusOne))]
    public void NPlusOne()
    {
        Enumerable.Range(0, 10).Foreach(_ => DbQueryUsers());
        Enumerable.Range(0, 10).Foreach(_ => DbQueryAccounts());
        Enumerable.Range(0, 80).Foreach(_ => DbQueryRoles());
        Enumerable.Range(0, 70).Foreach(_ => DbQueryGroups());
    }

    [HttpGet]
    [Route(nameof(QueryOptimization))]
    public async Task QueryOptimization(bool produceFastQueries)
    {
        if (produceFastQueries)
        {
            await _queryOptimizationService.ProduceFastQueries();
        }

        await _queryOptimizationService.ProduceSlowQueries();
    }
    
    [HttpGet]
    [Route(nameof(HighNumberOfQueries))]
    public void HighNumberOfQueries()
    {
        Enumerable.Range(0, 1000).Foreach(_ => DbQueryUsers());
        Enumerable.Range(0, 1000).Foreach(_ => DbQueryAccounts());
        Enumerable.Range(0, 8000).Foreach(_ => DbQueryRoles());
        Enumerable.Range(0, 7000).Foreach(_ => DbQueryGroups());
    }

    
    [HttpGet]
    [Route(nameof(TriggerQueryOptimizationConsumer))]
    public async Task TriggerQueryOptimizationConsumer(bool produceFastQueries)
    {
        using var activity = Activity.StartActivity();
        await _messagePublisher.Publish(new QueryOptimizationEvent
        {
            ProduceFastQueries = produceFastQueries
        });
    }

    [HttpGet]
    [Route(nameof(CallWithRetry))]
    public async Task<HttpResponseMessage> CallWithRetry(bool error = false)
    {
        using (var httpClient = new HttpClient())
        {
            try
            {
                await httpClient.GetAsync("http://localhost:9753/SampleInsights/GenerateSpans?uniqueSpans=1");
            }
            catch (Exception e)
            {
            }

            Thread.Sleep(30000);

            var result = await httpClient.GetAsync("http://localhost:9753/SampleInsights/GenerateSpans?uniqueSpans=1");
            return result;
        }
    }

    [HttpGet]
    [Route(nameof(RetryOnError))]
    public void RetryOnError(bool error = false)
    {
        if (error)
        {
            throw new Exception("First error");
        }

        DbQueryUsers();
    }

    private static void DbQueryUsers()
    {
        using var activity = Activity.StartActivity(ActivityKind.Client);
        activity?.SetTag("db.statement", "select * from users");
    }

    private static void DbQueryAccounts()
    {
        using var activity = Activity.StartActivity(ActivityKind.Client);
        activity?.SetTag("db.statement", "select * from accounts where a = 1");
    }
    
    private static void DbQueryAccounts10ms1()
    {
        using var activity = Activity.StartActivity("DbQueryAccounts10ms1", ActivityKind.Client);
        Thread.Sleep(10);
        activity?.SetTag("db.statement", "select * from accounts where a = 1");
    }
    
    private static void DbQueryAccounts10ms2()
    {
        using var activity = Activity.StartActivity("DbQueryAccounts10ms2", ActivityKind.Client);
        Thread.Sleep(10);
        activity?.SetTag("db.statement", "select * from accounts where a = 4 b =5");
    }

    
    private static void DbQueryAccounts10ms3()
    {
        using var activity = Activity.StartActivity("DbQueryAccounts10ms3", ActivityKind.Client);
        Thread.Sleep(10);
        activity?.SetTag("db.statement", "select * from accounts where a = 7 b =8 c = 9");
    }

    
    private static void DbQueryAccounts2()
    {
        using var activity = Activity.StartActivity(ActivityKind.Client);
        activity?.SetTag("db.statement", "select * from someother where a = 4 b =5 c = 6");
    }

    private static void DbQueryUsersBottleNeck()
    {
        using var activity = Activity.StartActivity();
        activity?.SetTag("db.statement", "select * from users");
        Task.Delay(100);
    }

    private static void DbQueryAccountsBottleNeck()
    {
        using var activity = Activity.StartActivity();
        activity?.SetTag("db.statement", "select * from accounts where a = 1 b =2 c = 3");
        Task.Delay(100);
    }

    private static void DbQueryRoles()
    {
        using var activity = Activity.StartActivity(ActivityKind.Client);
        activity?.SetTag("db.statement", "select * from roles");
    }

    private static void DbQueryGroups()
    {
        using var activity = Activity.StartActivity(ActivityKind.Client);
        activity?.SetTag("db.statement", "select * from groups");
    }

    [HttpGet]
    [Route("Error")]
    public async Task Error()
    {
        using var activity = Activity.StartActivity("Error");
        
        await Task.Delay(TimeSpan.FromMilliseconds(1));
        
        if (Random.Next(1, 10) % 2 == 0)
            throw new ValidationException("random validation error");
        else
        {
            throw new InvalidOperationException("operation is not valid");
        }
    }

    private async Task ErrorA()
    {
        throw new ValidationException("random validation error");
    }

    [HttpGet]
    [Route("Error2")]
    public async Task Error2()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(1));

        throw new ValidationException("random validation error");
    }

    [HttpGet]
    [Route("SlowEndpoint")]
    public async Task SlowEndpoint([FromQuery] int extraLatency)
    {
        await Task.Delay(extraLatency);
    }

    [HttpGet]
    [Route("SpanBottleneck")]
    public async Task SpanBottleneck()
    {
        await (new DummyFile()).SpanBottleNeck1();
        await (new DummyFile()).SpanBottleNeck2();
    }

    [HttpGet]
    [Route("SpanBottleneck1")]
    public async Task SpanBottleneck1()
    {
        await InternalFromApi();
    }

    private async Task InternalFromApi()
    {
        using var activity2 = Activity.StartActivity("InternalFromApi", ActivityKind.Internal);
        await (new DummyFile()).SpanBottleNeck1();
        await (new DummyFile()).SpanBottleNeck2();
    }

    [HttpGet]
    [Route("OverloadingA1")]
    public async Task MethodOverloadingA([FromQuery] String name)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(11));
    }

    [HttpGet]
    [Route("OverloadingA2")]
    public async Task MethodOverloadingA([FromQuery] String name, [FromQuery] int[] ids)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(12));
    }

    [HttpGet]
    [Route("OverloadingA3")]
    public async Task MethodOverloadingA([FromQuery] String name, [FromQuery] String description,
        [FromQuery] long longId)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(13));
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

    [HttpGet]
    [Route("Spans")]
    public async Task Spans()
    {
        using var a = Activity.StartActivity();
        {
            Thread.Sleep(5);
        }

        void Action()
        {
            using var b = Activity.StartActivity("Local func activity");
            {
                Thread.Sleep(5);
            }
        }

        Action();
        var action = () =>
        {
            using var c = Activity.StartActivity("Anonymous activity");
            {
                Thread.Sleep(5);
            }
        };
        action();
        await Task.CompletedTask;
    }

    [HttpGet]
    [Route("ScaleFactor")]
    public void ScaleFactor([FromQuery] int extraLatency)
    {
        using var activity = Activity.StartActivity("external call");
        lock (Random)
        {
            Console.WriteLine(extraLatency);
            Thread.Sleep(extraLatency);
        }
    }
    
    
    [HttpGet]
    [Route("ChattyApiInsight")]
    public async Task ChattyApiInsight()
    {
        for (int i = 0; i < 10; i++)
        {
            await HttpClientCall();
        }
    }

    private async Task HttpClientCall()
    {
        using var activity = Activity.StartActivity("HttpClientCall", ActivityKind.Client);

        activity.AddTag("http.url", "http://url.com");
        activity.AddTag("http.method", "HTTP.GET");
        activity.AddTag("http.target", "http://url.com");
        activity.AddTag("http.route", "url.com");
        
        await Task.Delay(10);
    }

    [HttpGet]
    [Route("AddTag")]
    public async Task AddTag()
    {
        using var activity = Activity.StartActivity("AddTag");
        activity?.AddTag("sample.num", 1);
        activity?.AddTag("sample.bool", true);
        activity?.AddBaggage("sss", "dddd");
        activity?.AddEvent(new ActivityEvent("something"));
        await Task.Delay(1);
    }
    

    [HttpGet]
    [Route("SpanGenerator")]
    public async Task SpanGenerator([FromQuery] int count)
    {
        var maxConcurrency = 10;
        int i = 3000;
        System.Diagnostics.Activity.Current = null;
        while (count > 0)
        {
            using var ac = Activity.StartActivity($"dynamic_span_{++i}");
            await Task.Delay(50);
            count--;
        }

        // while (count>0)
        // {
        //     List<Task> tasks = new List<Task>();
        //     var concurrency = Math.Min(maxConcurrency,count);
        //     for (int j=0; j < concurrency; j++)
        //     {
        //         var num = ++i;
        //         Task t = Task.Run(async () =>
        //         {
        //             System.Diagnostics.Activity.Current = null; 
        //             using var ac = Activity.StartActivity($"dynamic_span_{num}");
        //             await Task.Delay(50);
        //         });
        //         tasks.Add(t);
        //         count--;
        //     }
        //     await Task.WhenAll(tasks);
        //     Console.WriteLine("remaining: "+count);
        //    
        // }
        // for (var i = 0; i < count; i++)
        // {
        //     using var activity = Activity.StartActivity($"dynamic_span_{i+1}");
        //     await Task.Delay(50);
        // }
    }
    
    /// <summary>
    /// Should be called with 1 ms delay, after endpoint is processes increase to 100.
    /// This increase will be detected, and SlowDownSourceSpan span will be marked as a slowdown source
    /// </summary>
    /// <param name="delay"></param>
    [HttpGet]
    [Route("EndpointSlowDownSource")]
    public async Task EndpointSlowdownSourceEndpoint([FromQuery] int delay)
    {
        await Task.Delay(50);
        await SlowDownSourceSpan(delay);
    }

    private async Task SlowDownSourceSpan(int delay)
    {
        using var ac = Activity.StartActivity($"SlowDownSourceSpan");

        await Task.Delay(delay);
    }
}