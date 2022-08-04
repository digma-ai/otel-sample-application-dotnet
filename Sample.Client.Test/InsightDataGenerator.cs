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

    
    public async Task GenerateInsightData()
    {
        Console.WriteLine("***** START GenerateInsightData *****");

        Console.WriteLine("***** generate errors source *****");
        for (int i = 0; i < 10; i++)
        {
            HttpResponseMessage response = await _client.GetAsync($"{_url}/SampleInsights/ErrorSource");
            Console.WriteLine(response.StatusCode);
        }
        Console.WriteLine("***** generate errors insights *****");
        for (int i = 0; i < 10; i++)
        {
            HttpResponseMessage response = await _client.GetAsync($"{_url}/SampleInsights/Error");
            Console.WriteLine(response.StatusCode);
        }
        
        Console.WriteLine("***** generate slow endpoint insight *****");
        for (int i = 0; i < 2; i++)
        {
            HttpResponseMessage response = await _client.GetAsync($"{_url}/SampleInsights/SlowEndpoint?extraLatency=5000");
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
        
        Console.WriteLine("***** generate endpoints with method overloading A1 *****");
        for (int i = 0; i < 5; i++)
        {
            HttpResponseMessage response = await _client.GetAsync($"{_url}/SampleInsights/OverloadingA1?name=someName{i}");
            Console.WriteLine(response.StatusCode);
        }
        Console.WriteLine("***** generate endpoints with method overloading A2 *****");
        for (int i = 0; i < 9; i++)
        {
            HttpResponseMessage response = await _client.GetAsync($"{_url}/SampleInsights/OverloadingA2?name=someName{i}&ids={i + 1}&ids={i + 102}");
            Console.WriteLine(response.StatusCode);
        }
        Console.WriteLine("***** generate endpoints with method overloading A3 *****");
        for (int i = 0; i < 3; i++)
        {
            HttpResponseMessage response = await _client.GetAsync($"{_url}/SampleInsights/OverloadingA3?name=someName{i}&description=someDesc{i}&longId={i + 1000}");
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
        
        Console.WriteLine("***** END GenerateInsightData *****");

    }
}