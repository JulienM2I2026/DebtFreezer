using Gateway.Dtos;
using Gateway.Filter;
using Gateway.RestClient;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Gateway.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [RequireValidToken]
    public class PaymentController : ControllerBase
    {
        private readonly Client<PaymentDto, CreatePaymentDto> _client;
        private readonly IConfiguration _config;

        public PaymentController(IConfiguration config)
        {
            _config = config;
            var PaymentServiceUrl = _config["ServiceUrls:PaymentService"];
            _client = new Client<PaymentDto, CreatePaymentDto>($"{PaymentServiceUrl}/api/v1/Payment");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            Console.WriteLine("Gateway Payment GetAll");
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                return Unauthorized();

            var userId = claim.Value;

            var payments = await _client.GetRequestList($"?userId={userId}");
            Console.WriteLine(JsonSerializer.Serialize(payments));

            return Ok(payments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _client.GetRequest("/" + id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDto receive)
        {
            var result = await _client.PostRequest("", receive);
            Console.WriteLine("juste avant le renvoi au web");
            Console.WriteLine(JsonSerializer.Serialize(result)); // voir toutes les propriétés
            return Ok(result);
        }
    }
}
