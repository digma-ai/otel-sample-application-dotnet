using System.Diagnostics;

namespace Sample.MoneyTransfer.API.Domain.Services;


public interface IQueryOptimizationService
{
    Task ProduceFastQueries();

    Task ProduceSlowQueries();
}

public class QueryOptimizationService : IQueryOptimizationService
{
    private static readonly ActivitySource Activity = new(nameof(QueryOptimizationService));
    
    public Task ProduceFastQueries()
    {
        for (int i = 0; i < 100; i++)
        {
            using var fastActivity = Activity.StartActivity($"fast-query-{i}", ActivityKind.Client);
            fastActivity?.SetTag("db.statement", $"select * from fast_query_{i} where a = 1 AND b = 2 AND c = 3");
            fastActivity?.SetTag("db.name", "dotnet-sample");
            fastActivity?.SetTag("db.system", "postgresql");
        }

        return Task.CompletedTask;
    }
    
    public async Task ProduceSlowQueries()
    {
        using (var activity = Activity.StartActivity(ActivityKind.Client))
        {
            activity?.SetTag("db.statement", "select * from query_optimization where a = 1 AND b = 2 AND c = 3");
            activity?.SetTag("db.name", "dotnet-sample");
            activity?.SetTag("db.system", "postgresql");
            await Task.Delay(10000);
        }

        using (var nextActivity = Activity.StartActivity(ActivityKind.Client))
        {
            nextActivity?.SetTag("db.statement", "select * from one_more_query where a = 1 AND b = 2 AND c = 3");
            nextActivity?.SetTag("db.name", "dotnet-sample");
            nextActivity?.SetTag("db.system", "postgresql");
            await Task.Delay(10000);
        }
    }
}