using System.Diagnostics;

namespace Sample.NotificationService.API.Services;

public class MonitorHostedService:IHostedService
{
    private static readonly ActivitySource Activity = new(nameof(MonitorHostedService));
    private CancellationTokenSource _cts;
    private Task?  _executingTask;

    public Task StartAsync(CancellationToken cancellationToken)
    {

        // Create a cancellation token source
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // Start the background task
        _executingTask = Task.Run(() => DoWorkAsync(_cts.Token), _cts.Token);

        return Task.CompletedTask;
    }

    private async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using (Activity.StartActivity("Monitor"))
            {
                Console.WriteLine($"{DateTime.Now} monitoring..");
                await Task.Delay(300);
            }

            await Task.Delay(1000, cancellationToken);
        }
    }
    
  
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();

        // Wait for the background task to complete
        if (_executingTask != null)
        {
            try
            {
                await _executingTask;
            }
            catch (TaskCanceledException)
            {
                // Task was cancelled
            }
        }
    }
}