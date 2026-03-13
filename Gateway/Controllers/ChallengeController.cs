using Gateway.Dtos;
using Gateway.Filter;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [RequireValidToken]
    public class ChallengeController : ControllerBase
    {
        private const string ChallengeServiceUrl = "http://localhost:5065/api/Challenge";

        private readonly JsonSerializerOptions _jsonOpts = new() { PropertyNameCaseInsensitive = true };

        // Convertit un Guid en int positif de façon déterministe (workaround : ChallengeService attend un int)
        private static int GuidToUserId(string guidStr)
        {
            if (!Guid.TryParse(guidStr, out var guid)) return 1;
            var value = Math.Abs(BitConverter.ToInt32(guid.ToByteArray(), 0));
            return value == 0 ? 1 : value;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? GuidToUserId(claim.Value) : 1;
        }

        // GET /api/challenge — liste tous les challenges
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            using var http = new HttpClient();
            var response = await http.GetAsync(ChallengeServiceUrl);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, content);
            return Content(content, "application/json");
        }

        // GET /api/challenge/{id} — récupère un challenge par son ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using var http = new HttpClient();
            var response = await http.GetAsync($"{ChallengeServiceUrl}/{id}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, content);
            return Content(content, "application/json");
        }

        // POST /api/challenge — crée un challenge, injecte creatorUserId depuis le token
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChallengeGatewayDto dto)
        {
            var userId = GetCurrentUserId();

            var internal_dto = new CreateChallengeInternalDto
            {
                Title = dto.Title,
                Description = dto.Description,
                TargetAmount = dto.TargetAmount,
                DueDate = dto.DueDate,
                CreatorUserId = userId
            };

            using var http = new HttpClient();
            var json = new StringContent(JsonSerializer.Serialize(internal_dto), Encoding.UTF8, "application/json");
            var response = await http.PostAsync(ChallengeServiceUrl, json);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, content);
            return Content(content, "application/json");
        }

        // POST /api/challenge/{id}/join — rejoint un challenge avec l'userId du token
        [HttpPost("{id}/join")]
        public async Task<IActionResult> Join(int id)
        {
            var userId = GetCurrentUserId();
            var body = new { UserId = userId };

            using var http = new HttpClient();
            var json = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await http.PostAsync($"{ChallengeServiceUrl}/{id}/join", json);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, content);
            return Content(content, "application/json");
        }

        // POST /api/challenge/{id}/payment — enregistre un paiement dans le challenge
        [HttpPost("{id}/payment")]
        public async Task<IActionResult> RecordPayment(int id, [FromBody] RecordChallengePaymentGatewayDto dto)
        {
            var userId = GetCurrentUserId();
            var body = new { UserId = userId, PaymentId = dto.PaymentId };

            using var http = new HttpClient();
            var json = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await http.PostAsync($"{ChallengeServiceUrl}/{id}/payment", json);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, content);
            return Content(content, "application/json");
        }

        // GET /api/challenge/{id}/progress — progression collective du challenge
        [HttpGet("{id}/progress")]
        public async Task<IActionResult> GetProgress(int id)
        {
            using var http = new HttpClient();
            var response = await http.GetAsync($"{ChallengeServiceUrl}/{id}/progress");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, content);
            return Content(content, "application/json");
        }

        // GET /api/challenge/{id}/leaderboard — classement des participants
        [HttpGet("{id}/leaderboard")]
        public async Task<IActionResult> GetLeaderboard(int id)
        {
            using var http = new HttpClient();
            var response = await http.GetAsync($"{ChallengeServiceUrl}/{id}/leaderboard");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, content);
            return Content(content, "application/json");
        }
    }
}
