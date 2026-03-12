using System.Text.Json;

namespace ChallengeService.RestClient
{
    /// <summary>
    /// Client HTTP générique pour appeler les autres microservices.
    /// </summary>
    public class ServiceClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ServiceClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<T?> GetAsync<T>(string clientName, string path)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            var response = await client.GetAsync(path);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return default;

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        public async Task<List<T>> GetListAsync<T>(string clientName, string path)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            var response = await client.GetAsync(path);

            if (!response.IsSuccessStatusCode)
                return new List<T>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(json, _jsonOptions) ?? new List<T>();
        }
    }
}
