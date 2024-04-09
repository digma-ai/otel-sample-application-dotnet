using System.Diagnostics;
using MassTransit;
using Sample.MoneyTransfer.API.Domain.Services;

namespace Sample.MoneyTransfer.API.Consumer;

public class QueryOptimizationEvent
{
    public bool ProduceFastQueries { get; set; }
}

public class QueryOptimizationEventConsumer : IConsumer<QueryOptimizationEvent>
{
    private static readonly ActivitySource Activity = new(nameof(QueryOptimizationEventConsumer));
    private readonly IQueryOptimizationService _queryOptimizationService;

    public QueryOptimizationEventConsumer(IQueryOptimizationService queryOptimizationService)
    {
        _queryOptimizationService = queryOptimizationService;
    }

    public async Task Consume(ConsumeContext<QueryOptimizationEvent> context)
    {
        using var activity = Activity.StartActivity("QueryOptimizationEventConsumer Consume");

        if (context.Message.ProduceFastQueries)
        {
            await _queryOptimizationService.ProduceFastQueries();
        }

        await _queryOptimizationService.ProduceSlowQueries();
    }
}