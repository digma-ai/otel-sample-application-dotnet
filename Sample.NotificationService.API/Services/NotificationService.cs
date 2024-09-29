namespace Sample.NotificationService.API.Services;

public interface INotificationService
{
    Task SendTransferSuccessNotificationAsync(long accountId, decimal amount, DateTime transferDate);
    Task SendTransferFailureNotificationAsync(long accountId, string reason);
}

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendTransferSuccessNotificationAsync(long accountId, decimal amount, DateTime transferDate)
    {
        // Simulate sending a notification (e.g., via email or SMS)
        _logger.LogInformation($"Sending success notification to account {accountId} for transfer of {amount} at {transferDate}");
        await Task.CompletedTask; // Simulate async operation
    }

    public async Task SendTransferFailureNotificationAsync(long accountId, string reason)
    {
        // Simulate sending a failure notification
        _logger.LogInformation($"Sending failure notification to account {accountId}. Reason: {reason}");
        await Task.CompletedTask; // Simulate async operation
    }
}