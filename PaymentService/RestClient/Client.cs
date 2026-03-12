using System.Text;
using System.Text.Json;

namespace PaymentService.RestClient
{
    public class Client<TSent, TPost>
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public Client(string baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
        }

        public async Task<TSent> GetRequest(string url)
        {
            Console.WriteLine("_baseUrl: ");
            Console.WriteLine(_baseUrl);
            var response = await _httpClient.GetAsync(_baseUrl + url);
            if (!response.IsSuccessStatusCode) throw new Exception("Error while fetching ressource");

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TSent>(json, _jsonOptions);

            return result ?? throw new Exception("Result null");
        }

        public async Task<List<TSent>> GetRequestList(string url)
        {
            var response = await _httpClient.GetAsync(_baseUrl + url);
            if (!response.IsSuccessStatusCode) throw new Exception("Error while fetching ressource");

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<TSent>>(json, _jsonOptions);

            return result ?? throw new Exception("Result null");
        }

        public async Task<TSent> PostRequest(string url, TPost postElement)
        {
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(postElement),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(_baseUrl + url, jsonContent);
            if (!response.IsSuccessStatusCode) throw new Exception("Error while fetching ressource");

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TSent>(json, _jsonOptions);

            return result ?? throw new Exception("Result null");
        }
    }
}