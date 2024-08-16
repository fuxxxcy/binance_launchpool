using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace binance_launchpool
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();
    
        public static async Task Main(string[] args)
        {
            string apiKey = "api_key"; // тут api key
            string apiSecret = "api_secret"; // тут api secret
            
            string baseUrl = "https://api.binance.com";
            string endpoint = "/sapi/v1/simple-earn/locked/subscribe";
            
            string projectId = "project id"; // сюда вставляете project id
            decimal amount = 0; // сюда вписываете количество монет
            bool autoSubscribe = false;
            string sourceAccount = "ALL";   
            
            client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey);
            
            while (true)
            {
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var parameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("projectId", projectId),
                    new KeyValuePair<string, string>("amount", amount.ToString()),
                    new KeyValuePair<string, string>("autoSubscribe", autoSubscribe.ToString().ToLower()),
                    new KeyValuePair<string, string>("sourceAccount", sourceAccount),
                    new KeyValuePair<string, string>("timestamp", timestamp.ToString())
                };

                string queryString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
                string signature = CreateSignature(apiSecret, queryString);
                parameters.Add(new KeyValuePair<string, string>("signature", signature));

                HttpResponseMessage response = await client.PostAsync($"{baseUrl}{endpoint}", new FormUrlEncodedContent(parameters));
            
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
        }
    
        private static string CreateSignature(string secret, string queryString)
        {
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(queryString));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
