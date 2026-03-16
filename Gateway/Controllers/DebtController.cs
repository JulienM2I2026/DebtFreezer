using Gateway.Dtos;
using Gateway.Filter;
using Gateway.RestClient;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Gateway.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [RequireValidToken]
    public class DebtController : ControllerBase
    {
        private readonly Client<DebtSend, DebtReceive> _client;
        private readonly string _debtServiceUrl;

        public DebtController(IConfiguration config)
        {
            _debtServiceUrl = $"{config["ServiceUrls:DebtService"]}/api/v1/Debt";
            _client = new Client<DebtSend, DebtReceive>(_debtServiceUrl);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return Unauthorized();

            var debts = await _client.GetRequestList($"?userId={claim.Value}");
            return Ok(debts);
        }

        // GET /api/v1/Debt/paged?page=1&pageSize=10&status=0&type=1&creditor=bank
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] int? status = null, [FromQuery] int? type = null, [FromQuery] string? creditor = null)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return Unauthorized();

            using var http = new HttpClient();
            var qs = $"?userId={claim.Value}&page={page}&pageSize={pageSize}";
            if (status.HasValue) qs += $"&status={status}";
            if (type.HasValue) qs += $"&type={type}";
            if (!string.IsNullOrWhiteSpace(creditor)) qs += $"&creditor={Uri.EscapeDataString(creditor)}";

            var response = await http.GetAsync($"{_debtServiceUrl}/paged{qs}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, content);
            return Content(content, "application/json");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _client.GetRequest("/" + id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DebtReceive receive)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return Unauthorized("UserId manquant dans le token");

            receive.UserId = Guid.Parse(claim.Value);
            var result = await _client.PostRequest("", receive);
            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] UpdateDebtDto dto)
        {
            using var http = new HttpClient();
            var json = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_debtServiceUrl}/{id}") { Content = json };
            var response = await http.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, content);
            return Content(content, "application/json");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _client.DeleteRequest($"/{id}");
            return NoContent();
        }

        // GET /api/v1/Debt/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return Unauthorized();

            using var http = new HttpClient();
            var response = await http.GetAsync($"{_debtServiceUrl}/summary?userId={claim.Value}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, content);
            return Content(content, "application/json");
        }

        // GET /api/v1/Debt/by-month
        [HttpGet("by-month")]
        public async Task<IActionResult> GetByMonth()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return Unauthorized();

            using var http = new HttpClient();
            var response = await http.GetAsync($"{_debtServiceUrl}/by-month?userId={claim.Value}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, content);
            return Content(content, "application/json");
        }
    }
}
