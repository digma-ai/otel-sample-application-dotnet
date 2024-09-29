namespace Sample.NotificationService.API.Contracts;

public class TransferNotificationRequest
{
    public long AccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransferDate { get; set; }
}

public class FailureNotificationRequest
{
    public long AccountId { get; set; }
    public string Reason { get; set; }
}