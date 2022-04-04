using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sample.MoneyTransfer.API.Data;
using Sample.MoneyTransfer.API.DTO;

namespace Sample.MoneyTransfer.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TransferController : ControllerBase
{
    private static readonly ActivitySource Activity = new(nameof(TransferController));
    private readonly MoneyKeepingContext moneyVault;
    private readonly ILogger<TransferController> _logger;

    public TransferController(MoneyKeepingContext moneyVault,  ILogger<TransferController> logger)
    {
        this.moneyVault = moneyVault;
        _logger = logger;
    }

    [HttpGet(Name = "deposit")]
    public TransferResult TransferFunds(int accountId, int amount)
    {

        using (var activity = Activity.StartActivity("Process transfer", ActivityKind.Internal)){

            var account = moneyVault.Accounts.Find(accountId);



            
        }
        return new TransferResult();

    }
}

