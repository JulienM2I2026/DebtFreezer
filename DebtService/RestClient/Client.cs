using System.Net.Http.Headers;
using System.Text.Json;

namespace DebtService.RestClient
{
    public class Client<TSent>
    {

        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public Client(string baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
        }

        public async Task<TSent> GetRequest(string url)
        {
            var response = await _httpClient.GetAsync(_baseUrl + url);
            if (!response.IsSuccessStatusCode) throw new Exception("Error while fetching ressource");

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TSent>(json);
            return result ?? throw new Exception("Result null");
        }

        public async Task<TSent> GetRequestWithToken(string url, string token)
        {
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync(_baseUrl + url);
            if (!response.IsSuccessStatusCode) throw new Exception("Error while fetching ressource");

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TSent>(json);
            return result ?? throw new Exception("Result null");
        }
    }
}
