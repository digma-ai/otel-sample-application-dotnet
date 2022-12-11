using ClientTester;


var url = "http://localhost:7151";
var value = Environment.GetEnvironmentVariable("MoneyTransferApiUrl");
if (value != null)
{
    url = value;
}
else if(args.Any())
{
    url = args[0];
}

var accountapi = new AccountsApiClient(url);
var transfersapi = new TransfersApiClient(url);

await new InsightDataGenerator(url).GenerateInsightData();

// await new InsightDataGenerator(url).GenerateDurationData(TimeSpan.FromMinutes(10), 10);

// await new InsightDataGenerator(url).GenerateGoodScalingData();
// await new InsightDataGenerator(url).GenerateBadScalingData();
// await new InsightDataGenerator(url).GenerateNoScalingData();

var externalAccounts = await accountapi.Create(10,10, false);

foreach (var account in externalAccounts)
{
    var transfer = await transfersapi.Transfer(account.AccountId,
        externalAccounts.ElementAt(0).AccountId, 30);
}

Console.WriteLine("Transfer Done!");

//Create error hotspot
new DataScenarios(accountapi, transfersapi).TransferFromUnexistingAccounts(30);

var accounts = await accountapi.Create(10, 10, false);

Console.WriteLine("Accounts Created!");


//create usage high

for (var i = 0; i < 300; i++)
{
    await accountapi.Get(accounts.ElementAt(1).AccountId);
    await accountapi.Get(accounts.ElementAt(2).AccountId);

}

