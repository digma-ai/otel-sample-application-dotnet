using System;
using System.Text.Json;

namespace ClientTester
{
	public class Utils
	{
        public static  async Task<T> ParseResponse<T>(HttpResponseMessage httpResponse)
        {
          
            //var contentStream = await httpResponse.Content.ReadAsStreamAsync();
            var response = await httpResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            T result = JsonSerializer.Deserialize<T>(response, options);


            return result;

            

        }
    }
}

