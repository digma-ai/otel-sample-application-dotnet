using System.Diagnostics;
using MassTransit;
using Sample.MoneyTransfer.API.Domain.Models;

namespace Sample.MoneyTransfer.API.Consumer;

public class TransferFundsEvent
{
    public TransferRecord TransferRecord { get; set; }
    public int DelayInMS { get; set; }
}

public class TransferFundsEventConsumer : IConsumer<TransferFundsEvent>
{
    private static readonly ActivitySource Activity = new(nameof(TransferFundsEventConsumer));

    public async Task Consume(ConsumeContext<TransferFundsEvent> context)
    {
        using var activity = Activity.StartActivity("transfer funds event consumer");
        Console.Write($"Consume: {context.Message.TransferRecord.Id}");
        await Task.Delay(context.Message.DelayInMS);
    }
}