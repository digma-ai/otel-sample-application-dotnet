using ClientTester;

string URL = "http://localhost:7151";

if (args.Count() > 0)
{
    URL = args[0];
}

var accountapi = new AccountsApiClient(URL);
var transfersapi = new TransfersApiClient(URL);


var externalAccounts = await accountapi.Create(10,10, false);

foreach (var account in externalAccounts)
{
    var transfer = await transfersapi.Transfer(account.AccountId,
        externalAccounts.ElementAt(0).AccountId, 30);

}

//Create error hotspot
new DataScenarios(accountapi, transfersapi).TransferFromUnexistingAccounts(30);

var accounts = await accountapi.Create(10, 10, false);


//create usage high

for (var i = 0; i < 300; i++)
{
    await accountapi.Get(accounts.ElementAt(1).AccountId);
    await accountapi.Get(accounts.ElementAt(2).AccountId);


}
