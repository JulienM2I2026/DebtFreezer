using Gateway.Dtos;
using Gateway.Filter;
using Gateway.RestClient;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [RequireValidToken]
    public class DebtController : ControllerBase
    {
        private readonly Client<DebtSend, DebtReceive> _client;


        public DebtController()
        {
            _client = new Client<DebtSend, DebtReceive>("http://localhost:5193/api/Debt");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _client.GetRequestList(""));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _client.GetRequest("/" + id));
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DebtReceive receive)
        {
            return Ok(await _client.PostRequest("", receive));
        }
    }
}
