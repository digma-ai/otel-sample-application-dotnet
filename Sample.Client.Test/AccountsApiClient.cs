using System.Text;
using System.Text.Json;
using ClientTester;
using Sample.MoneyTransfer.API.DTO;

public class AccountsApiClient{

    private HttpClient client;
    private readonly string url;

    public AccountsApiClient(string url)
    {
        client = new HttpClient();
        this.url = url;
    }





    public async Task<AccountDto> Get(long accountId)
    {
        HttpResponseMessage? response = await client.GetAsync($"{url}/Account/{accountId}");
        if (response.IsSuccessStatusCode)
        {
            return await Utils.ParseResponse<AccountDto>(response);

        }
        return null;

    }
    public async Task<IEnumerable<CreateAccountResponse>> Create(int number, int initialFunds, bool isInternal)
    {
        List<CreateAccountResponse> accounts = new List<CreateAccountResponse>();

        for (int i=0; i<number; i++)
        {

            var request = new NewAccountRequest() { AccountName = $"account{i}",InitialFunds=initialFunds, Internal=isInternal  };

            string jsonString = JsonSerializer.Serialize(request);

            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage? response = await client.PostAsync($"{url}/Account", content);

            if (response.IsSuccessStatusCode)
            {
                accounts.Add(await Utils.ParseResponse<CreateAccountResponse>(response));
            }
        }

        return accounts;

    }
}

//create account

