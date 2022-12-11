using System;
using System.Diagnostics;
using OpenTelemetry.Instrumentation.Digma.Helpers.Attributes;
using Sample.MoneyTransfer.API.Data;
using Sample.MoneyTransfer.API.Domain.Models;

namespace Sample.MoneyTransfer.API.Domain.Services
{
    [ActivitiesAttributes("activity_type:Money Operations", "layer:Domain")]
    public class MoneyTransferDomainService : IMoneyTransferDomainService
    {
        private static readonly ActivitySource Activity = new(nameof(MoneyTransferDomainService));

        private readonly Gringotts  moneyVault;
        private readonly ICreditProviderService creditProviderService;

        [TraceActivity(name:"Validating account funds")]
        private async Task<bool> ValidateAccountFunds(Account account, int amount)
        {

                if (account.Balance >= amount)
                {
                    return true;
                }
                else
                {
                    if (account.Internal)
                    {
                        return false;
                    }
                    else
                    {
                        return await creditProviderService.CheckCreditProvider(account.Id);
                    }
                }
                
        }
        
        [TraceActivity(recordExceptions:false)]
        private async Task<Account> RetrieveAccount(long id)
        {
            return await moneyVault.Accounts.FindAsync(id);
        }

        public async Task<TransferRecord> TransferFunds(long source, long target, int amount)
        {
            var sourceAccount = await RetrieveAccount(target);

            var targetAccount = await RetrieveAccount(target);

            bool fundsAvailalbe = await ValidateAccountFunds(sourceAccount, amount);

            if (!fundsAvailalbe)
            {
                throw new Exception("Insufficient funds");
            }

            using (var activity = Activity.StartActivity("Peristing balance transfer", ActivityKind.Internal))
            {

                sourceAccount.Balance -= amount;
                targetAccount.Balance += amount;
                _ = await moneyVault.SaveChangesAsync();

            }

            using (var activity = Activity.StartActivity("Creating record of transaction", ActivityKind.Internal))
            {
                var transferRecord = new TransferRecord()
                {
                    SourceAccount = sourceAccount,
                    TargetAccount = targetAccount,
                    TransferTime = DateTime.Now,
                    Amount = amount
                };
                moneyVault.Add<TransferRecord>(transferRecord);
                _ = await moneyVault.SaveChangesAsync();
                return transferRecord;

            }
        }

        public async Task DepositeFunds(long accountId, int amount)
        {
            
            using (var activity = Activity.StartActivity("Peristing balance increase", ActivityKind.Internal))
            {
                var account = await RetrieveAccount(accountId);
                
                account.Balance += amount;
                _ = await moneyVault.SaveChangesAsync();

                if (account.Internal==false){
                    using (var testActivity = Activity.StartActivity("confirm new balance", ActivityKind.Internal))
                    {
                        this.ConfirmTransaction();
                    }
                }
                    
            }

  
        }

     

        public MoneyTransferDomainService(Gringotts  context, ICreditProviderService creditProviderService)
        {
            this.moneyVault = context;
            this.creditProviderService = creditProviderService;
        }

   private void ConfirmTransaction()
        {
        }
    }
}