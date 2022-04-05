using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sample.MoneyTransfer.API.Data;
using Sample.MoneyTransfer.API.Domain.Services;
using Sample.MoneyTransfer.API.DTO;

namespace Sample.MoneyTransfer.API.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TransferController : ControllerBase
{
    private static readonly ActivitySource Activity = new(nameof(TransferController));
    private readonly MoneyKeepingContext moneyVault;
    private readonly ILogger<TransferController> _logger;
    private readonly IMoneyTransferDomainService moneyTransferDomainService;

    public TransferController(MoneyKeepingContext moneyVault,
                              ILogger<TransferController> logger,
                              IMoneyTransferDomainService moneyTransferDomainService)
    {
        this.moneyVault = moneyVault;
        _logger = logger;
        this.moneyTransferDomainService = moneyTransferDomainService;
    }

    [HttpPost(Name = "deposit")]
    public TransferResult DepositFunds(DepositRequest request)
    {

        using (var activity = Activity.StartActivity("Process deposit", ActivityKind.Internal))
        {

            var account = moneyVault.Accounts.Find(request.AccountId);
            moneyTransferDomainService.DepositeFunds(account,
                                                     request.Amount);


        }
        return new TransferResult();

    }
    [HttpPost(Name = "transfer")]
    public TransferResult TransferFunds(TransferRequest request)
    {

        using (var activity = Activity.StartActivity("Process transfer", ActivityKind.Internal)){

            var sourceAccount = moneyVault.Accounts.Find(request.SouceAccountId);
            var targetAccount = moneyVault.Accounts.Find(request.TargetAccountId);
            moneyTransferDomainService.TransferFunds(sourceAccount,
                                                     targetAccount,
                                                     request.Amount);


        }
        return new TransferResult();

    }
}

