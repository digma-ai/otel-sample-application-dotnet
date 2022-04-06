using System;
namespace ClientTester
{
	public class DataScenarios
	{
        private readonly AccountsApiClient accountsApi;
        private readonly TransfersApiClient transfersApi;

        public DataScenarios(AccountsApiClient accountsApi, TransfersApiClient transfersApi)
		{
            this.accountsApi = accountsApi;
            this.transfersApi = transfersApi;
        }

        public void TransferFromUnexistingAccounts(int iterations)
        {
            for (int i =0;  i < iterations; i++)
            {
                transfersApi.Transfer(-1, -2, 30);
            }
               
        }
	}
}

