using System.Text;
using System.Text.Json;
using ClientTester;
using Sample.MoneyTransfer.API.DTO;

public class TransfersApiClient
{

    private HttpClient client;
    private readonly string url;

    public TransfersApiClient(string url)
    {
        client = new HttpClient();
        this.url = url;
    }



    public async Task<TransferResult> Transfer(long sourceAccount,
        long targetAccount, int amount)
    {

        var request = new TransferRequest() { SouceAccountId = sourceAccount,
                                              TargetAccountId =targetAccount,
                                              Amount = amount};

        string jsonString = JsonSerializer.Serialize(request);

        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        HttpResponseMessage? response = await client.PostAsync($"{url}/Transfer/TransferFunds", content);
        if (response.IsSuccessStatusCode)
        {
            return await Utils.ParseResponse<TransferResult>(response);

        }

        return null;



    }
}

//create account

