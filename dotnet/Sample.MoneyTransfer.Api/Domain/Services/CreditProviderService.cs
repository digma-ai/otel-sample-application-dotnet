using System;
using System.Diagnostics;

namespace Sample.MoneyTransfer.API.Domain.Services
{
    public class CreditProviderService : ICreditProviderService
    {
        private static readonly ActivitySource Activity = new(nameof(MoneyTransferDomainService));

        Random rand = new Random();
        public async Task<bool> CheckCreditProvider(long accountId)
        {
            using (var activity = Activity.StartActivity("Validating account funds", ActivityKind.Internal))
            {
                //Simulate check with external service
                await Task.Delay(300);
                return (rand.Next() % 5) != 0;

            }


        }
    }
}

