using System;
using System.Text.Json;

namespace ClientTester
{
	public static class Utils
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

        public static DateTime RoundSeconds(this DateTime c)
        {
            return new DateTime(c.Year, c.Month, c.Day, c.Hour, c.Minute, c.Second, 0, c.Kind);
        }
    }
}

