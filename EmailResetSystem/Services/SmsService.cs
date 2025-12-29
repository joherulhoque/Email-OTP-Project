using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace ResetSystem.Services
{
    public class SmsService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public SmsService(IHttpClientFactory factory, IConfiguration config)
        {
            _client = factory.CreateClient();
            _config = config;
        }

        public async Task<bool> SendSmsAsync(string mobile, string message)
        {
            var token = _config["Sms:GreenWebToken"];
            var url = _config["Sms:GreenWebUrl"];

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string,string>("token", token),
            new KeyValuePair<string,string>("to", mobile),
            new KeyValuePair<string,string>("message", message)
        });

            var res = await _client.PostAsync(url, content);
            return res.IsSuccessStatusCode;
        }
    }
}
