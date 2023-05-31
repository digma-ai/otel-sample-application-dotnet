using System.Diagnostics;

namespace Sample.MoneyTransfer.API.Controllers;

public class DummyFile
{
    private static readonly ActivitySource Activity = new(nameof(DummyFile));
    
    public async Task SpanBottleNeck1()
    {
        using var activity2 = Activity.StartActivity("SpanBottleneck 1");
        await Task.Delay(TimeSpan.FromMilliseconds(10));
    }
    
    public async Task SpanBottleNeck2()
    {
        using var activity2 = Activity.StartActivity("SpanBottleneck 2.1", ActivityKind.Client);
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        await InternalSpanBottleNeck();
    }

    public async Task InternalSpanBottleNeck()
    {
        // actual root cause
      //  using var activity2 = Activity.StartActivity("InternalSpanBottleNeck");
        await Task.Delay(TimeSpan.FromMilliseconds(200));
    }
}