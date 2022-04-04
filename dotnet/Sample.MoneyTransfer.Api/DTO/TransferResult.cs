namespace Sample.MoneyTransfer.API.DTO;

public class TransferResult
{
    public bool Success { get; set; }

    public DateTime Date { get; set; }

    public int Deposited { get; set; }

    public int Withdrawn {get; set;}

    public string[] Issues { get; set; }
}

