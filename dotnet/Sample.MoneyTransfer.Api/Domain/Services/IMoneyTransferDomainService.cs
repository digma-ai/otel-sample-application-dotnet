using Sample.MoneyTransfer.API.Domain.Models;

namespace Sample.MoneyTransfer.API.Domain.Services
{
    public interface IMoneyTransferDomainService
    {
        void DepositeFunds(Account account, int amount);
        void TransferFunds(Account source, Account target, int amount);
    }
}