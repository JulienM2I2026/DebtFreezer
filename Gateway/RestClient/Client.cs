using System.Text;
using System.Text.Json;

namespace Gateway.RestClient
{
    public class Client<TSent, TPost>
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


        public async Task<List<TSent>> GetRequestList(string url)
        {
            var response = await _httpClient.GetAsync(_baseUrl + url);
            if (!response.IsSuccessStatusCode) throw new Exception("Error while fetching ressource");

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<TSent>>(json);
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
            var result = JsonSerializer.Deserialize<TSent>(json);
            return result ?? throw new Exception("Result null");

        }
    }
}
