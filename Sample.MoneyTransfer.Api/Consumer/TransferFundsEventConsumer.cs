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
        using var activity = Activity.StartActivity();
        Console.Write($"Consume: {context.Message?.TransferRecord?.Id}");
       //await Task.Delay(context.Message.DelayInMS);
       // var client = new HttpClient();
       // HttpResponseMessage? response = await client.GetAsync($"http://localhost:7151/Account/1");
       // Console.WriteLine(response.StatusCode);

    }
}