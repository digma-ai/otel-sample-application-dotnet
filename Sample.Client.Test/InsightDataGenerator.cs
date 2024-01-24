using System.Collections.Concurrent;
using Humanizer;
using MathNet.Numerics.Distributions;

namespace ClientTester;

public class InsightDataGenerator
{
    private HttpClient _client;
    private readonly string _url;

    public InsightDataGenerator(string url)
    {
        _client = new HttpClient();
        _url = url;
    }

    public async Task GenerateUnverifiedChange()
    {
        Console.WriteLine("***** generate unverified change *****");

        var tasks = new ConcurrentBag<Task>();

        await Execute(() => tasks.Add(_client.GetAsync($"{_url}/SampleInsights/UnverifiedChange/10")), 16, 8.Seconds());
        await Execute(() => tasks.Add(_client.GetAsync($"{_url}/SampleInsights/UnverifiedChange/20")), 16, 4.Seconds());

        await Task.WhenAll(tasks);
    }

    public async Task GenerateSlowdownSourceData()
    {
        Console.WriteLine("***** generate slowdown source data *****");

        var tasks = new ConcurrentBag<Task>();

        await Execute(() => tasks.Add(_client.GetAsync($"{_url}/SampleInsights/EndpointSlowDownSource/1")), 30, 15.Seconds());
        await Task.WhenAll(tasks);

        // 30 secs
        await Task.Delay(30000);
        
        await Execute(() => tasks.Add(_client.GetAsync($"{_url}/SampleInsights/EndpointSlowDownSource/200")), 30, 15.Seconds());
        await Task.WhenAll(tasks);
    }
    
    public async Task GenerateNoScalingData()
    {
        Console.WriteLine("***** generate no scaling *****");

        var tasks = new ConcurrentBag<Task>();

        await Execute(() => tasks.Add(_client.GetAsync($"{_url}/SampleInsights/lock/100")), 5,
            TimeSpan.FromSeconds(20));
        await Execute(() => tasks.Add(_client.GetAsync($"{_url}/SampleInsights/lock1/100")), 5,
            TimeSpan.FromSeconds(20));
        await Execute(() => tasks.Add(_client.GetAsync($"{_url}/SampleInsights/lock2/100")), 5,
            TimeSpan.FromSeconds(20));
        await Execute(() => tasks.Add(_client.GetAsync($"{_url}/SampleInsights/lock3/100")), 5,
            TimeSpan.FromSeconds(20));
        await Execute(() => tasks.Add(_client.GetAsync($"{_url}/SampleInsights/lock4/50")), 5,
            TimeSpan.FromSeconds(20));
        await Execute(() => tasks.Add(_client.GetAsync($"{_url}/SampleInsights/lock5/50")), 5,
            TimeSpan.FromSeconds(20));
        await Execute(() => tasks.Add(_client.GetAsync($"{_url}/SampleInsights/lock6/50")), 5,
            TimeSpan.FromSeconds(20));

        await Task.WhenAll(tasks);
    }

    public async Task GenerateGoodScalingData()
    {
        Console.WriteLine("***** generate good scaling *****");

        var tasks = new ConcurrentBag<Task>();

        for (var i = 1; i < 20; i++)
        {
            await Execute(() => tasks.Add(_client.GetAsync($"{_url}/SampleInsights/lock/10")), i,
                TimeSpan.FromSeconds(1));
        }

        await Task.WhenAll(tasks);
    }

    public async Task GenerateBadScalingData()
    {
        Console.WriteLine("***** generate bad scaling *****");

        var tasks = new ConcurrentBag<Task>();

        for (var i = 1; i < 20; i++)
        {
            await Execute(() => tasks.Add(_client.GetAsync($"{_url}/SampleInsights/lock/100")), i,
                TimeSpan.FromSeconds(2));
        }

        await Task.WhenAll(tasks);
    }

    public async Task Execute(Action action, float timesPerSecond, TimeSpan duration)
    {
        Console.WriteLine($"Executing action {timesPerSecond} times per second, for {duration.Humanize()}");

        var safeMargin = 100;
        double mean = (1000 - safeMargin * 2) / (timesPerSecond);
        var poisson = new Poisson(mean);

        async Task SchedulerLoop()
        {
            await Task.Delay(1000 - DateTime.Now.Millisecond);
            Console.WriteLine($"{new DateTime(DateTime.Now.Ticks):O} started");
            var lastSecond = DateTime.Now.RoundSeconds();
            var counter = 0;
            var tokenSource = new CancellationTokenSource(duration);
            while (!tokenSource.Token.IsCancellationRequested)
            {
                action();
                counter++;
                try
                {
                    var delay = Math.Min(mean, poisson.Sample()).Milliseconds();
                    await Task.Delay(delay, tokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                finally
                {
                    var thisSecond = DateTime.Now.RoundSeconds();
                    if (lastSecond != thisSecond)
                    {
                        Console.WriteLine($"{lastSecond:s} executed {counter} times");
                        lastSecond = thisSecond;
                        counter = 0;
                    }
                }
            }

            Console.WriteLine($"{new DateTime(DateTime.Now.Ticks):O} ended");
        }

        await SchedulerLoop();
    }

    public async Task GenerateInsightData()
    {
        Console.WriteLine("***** START GenerateInsightData *****");

        await GenerateUnverifiedChange();

        await GenerateBadScalingData();

        Console.WriteLine("***** generate N-Plus-One insight *****");
        await _client.GetAsync($"{_url}/SampleInsights/NPlusOne");
        await _client.GetAsync($"{_url}/SampleInsights/NPlusOneSingleSpan");

        Console.WriteLine("***** generate high number of queries insights *****");
        await _client.GetAsync($"{_url}/SampleInsights/HighNumberOfQueries");
        
        Console.WriteLine("***** generate errors insights *****");
        for (int i = 0; i < 20; i++)
        {
            HttpResponseMessage response = await _client.GetAsync($"{_url}/SampleInsights/Error");
            Console.WriteLine(response.StatusCode);
        }

        Console.WriteLine("***** generate slow endpoint insight *****");
        for (int i = 0; i < 2; i++)
        {
            HttpResponseMessage response =
                await _client.GetAsync($"{_url}/SampleInsights/SlowEndpoint?extraLatency=5000");
            Console.WriteLine(response.StatusCode);
        }

        Console.WriteLine("***** generate slow bottleneck insight *****");
        for (int i = 0; i < 1; i++)
        {
            HttpResponseMessage response =
                await _client.GetAsync($"{_url}/SampleInsights/SpanBottleneck");
            Console.WriteLine(response.StatusCode);
        }

        Console.WriteLine("***** generate low usage insight *****");
        for (int i = 0; i < 1; i++)
        {
            HttpResponseMessage response =
                await _client.GetAsync($"{_url}/SampleInsights/LowUsage");
            Console.WriteLine(response.StatusCode);
        }

        Console.WriteLine("***** generate normal usage insight *****");
        for (int i = 0; i < 50; i++)
        {
            HttpResponseMessage response =
                await _client.GetAsync($"{_url}/SampleInsights/NormalUsage");
            Console.WriteLine(response.StatusCode);
        }
        
        Console.WriteLine("***** generate BottlwNeckTest *****");
        for (int i = 0; i < 50; i++)
        {
            HttpResponseMessage response =
                await _client.GetAsync($"{_url}/SampleInsights/BottlwNeckTest");
            Console.WriteLine(response.StatusCode);
        }

        Console.WriteLine("***** generate endpoints with method overloading A1 *****");
        for (int i = 0; i < 5; i++)
        {
            HttpResponseMessage response =
                await _client.GetAsync($"{_url}/SampleInsights/OverloadingA1?name=someName{i}");
            Console.WriteLine(response.StatusCode);
        }

        Console.WriteLine("***** generate endpoints with method overloading A2 *****");
        for (int i = 0; i < 9; i++)
        {
            HttpResponseMessage response =
                await _client.GetAsync(
                    $"{_url}/SampleInsights/OverloadingA2?name=someName{i}&ids={i + 1}&ids={i + 102}");
            Console.WriteLine(response.StatusCode);
        }

        Console.WriteLine("***** generate endpoints with method overloading A3 *****");
        for (int i = 0; i < 3; i++)
        {
            HttpResponseMessage response = await _client.GetAsync(
                $"{_url}/SampleInsights/OverloadingA3?name=someName{i}&description=someDesc{i}&longId={i + 1000}");
            Console.WriteLine(response.StatusCode);
        }

        Console.WriteLine("***** generate high usage insight *****");
        for (int i = 0; i < 400; i++)
        {
            HttpResponseMessage response =
                await _client.GetAsync($"{_url}/SampleInsights/HighUsage");
            Console.WriteLine(response.StatusCode);
        }

        await _client.GetAsync($"{_url}/SampleInsights/Spans");

        await _client.GetAsync($"{_url}/SampleInsights/ChattyApiInsight");

        Console.WriteLine("***** END GenerateInsightData *****");
    }
}