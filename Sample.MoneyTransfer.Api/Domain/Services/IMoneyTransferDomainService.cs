using Sample.MoneyTransfer.API.Domain.Models;

namespace Sample.MoneyTransfer.API.Domain.Services
{
    public interface IMoneyTransferDomainService
    {
        Task DepositeFunds(long account, int amount);
        Task<TransferRecord> TransferFunds(long source, long target, int amount);
    }
}