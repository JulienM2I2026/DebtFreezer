using Gateway.Dtos;
using Gateway.Filter;
using Gateway.RestClient;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [RequireValidToken]
    public class DebtController : ControllerBase
    {
        private readonly Client<DebtSend, DebtReceive> _client;
        private readonly IConfiguration _config;

        public DebtController(IConfiguration config)
        {
            _config = config;
            var authServiceUrl = _config["ServiceUrls:DebtService"];
            _client = new Client<DebtSend, DebtReceive>($"{authServiceUrl}/api/Debt");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                return Unauthorized();

            var userId = claim.Value;

            var debts = await _client.GetRequestList($"?userId={userId}");
            Console.WriteLine(JsonSerializer.Serialize(debts)); // voir toutes les propriétés

            return Ok(debts);
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

            if (claim == null)
            {
                return Unauthorized("UserId manquant dans le token");
            }

            var userId = Guid.Parse(claim.Value);
            receive.UserId = userId;
            var result = await _client.PostRequest("", receive);
            return Ok(result);
        }
    }
}
