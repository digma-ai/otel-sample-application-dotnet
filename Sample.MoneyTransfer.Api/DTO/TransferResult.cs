namespace Sample.MoneyTransfer.API.DTO;

public class TransferResult
{
    public bool Success { get; set; }

    public DateTime TransferDate { get; set; }

    public string[] Issues { get; set; }
}

