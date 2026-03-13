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
    public class StrategyController : ControllerBase
    {
        private readonly string _strategyServiceUrl;

        public StrategyController(IConfiguration config)
        {
            _strategyServiceUrl = $"{config["ServiceUrls:StrategyService"]}/api/Strategy";
        }

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && Guid.TryParse(claim.Value, out var guid) ? guid : Guid.Empty;
        }

        // POST /api/strategy/calculate — calcule et sauvegarde un plan, injecte l'userId depuis le token
        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate([FromBody] CalculateStrategyGatewayDto dto)
        {
            var userId = GetCurrentUserId();

            var internalDto = new CalculateStrategyInternalDto
            {
                UserId = userId,
                MonthlyBudget = dto.MonthlyBudget,
                StrategyType = dto.StrategyType
            };

            using var http = new HttpClient();
            var json = new StringContent(JsonSerializer.Serialize(internalDto), Encoding.UTF8, "application/json");
            var response = await http.PostAsync($"{_strategyServiceUrl}/calculate", json);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, content);
            return Content(content, "application/json");
        }

        // GET /api/strategy/plans — retourne les plans de l'utilisateur connecté
        [HttpGet("plans")]
        public async Task<IActionResult> GetUserPlans()
        {
            var userId = GetCurrentUserId();

            using var http = new HttpClient();
            var response = await http.GetAsync($"{_strategyServiceUrl}/plans/user/{userId}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, content);
            return Content(content, "application/json");
        }

        // GET /api/strategy/plans/{id} — retourne un plan par son ID
        [HttpGet("plans/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using var http = new HttpClient();
            var response = await http.GetAsync($"{_strategyServiceUrl}/plans/{id}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, content);
            return Content(content, "application/json");
        }

        // DELETE /api/strategy/plans/{id} — supprime un plan
        [HttpDelete("plans/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var http = new HttpClient();
            var response = await http.DeleteAsync($"{_strategyServiceUrl}/plans/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, content);
            }
            return NoContent();
        }
    }
}
