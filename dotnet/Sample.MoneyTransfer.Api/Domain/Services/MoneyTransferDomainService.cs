using System;
using System.Diagnostics;
using Sample.MoneyTransfer.API.Data;
using Sample.MoneyTransfer.API.Domain.Models;

namespace Sample.MoneyTransfer.API.Domain.Services
{
    public class MoneyTransferDomainService : IMoneyTransferDomainService
    {
        private static readonly ActivitySource Activity = new(nameof(MoneyTransferDomainService));

        private readonly MoneyKeepingContext context;

        public MoneyTransferDomainService(MoneyKeepingContext context)
        {
            this.context = context;
        }

        public async void TransferFunds(Account source, Account target, int amount)
        {
            using (var activity = Activity.StartActivity("Peristing new balance", ActivityKind.Internal))
            {

                source.Balance -= amount;
                target.Balance += amount;
                _ = await context.SaveChangesAsync();

            }

            using (var activity = Activity.StartActivity("Creating record of transaction", ActivityKind.Internal))
            {
                context.Add<TransferRecord>(new TransferRecord() { SourceAccount = source, TargetAccount = target });
                _ = await context.SaveChangesAsync();

            }


        }

        public async void DepositeFunds(Account account, int amount)
        {
            account.Balance += amount;
            _ = await context.SaveChangesAsync();
        }

    }
}