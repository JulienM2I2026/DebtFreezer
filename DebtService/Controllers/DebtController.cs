using DebtService.Dtos;
using DebtService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DebtService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebtController : ControllerBase
    {
        private readonly IService<DebtSend, DebtReceive> _service;

        public DebtController(IService<DebtSend, DebtReceive> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var debt = await _service.GetById(id);
            if (debt is null)
            {
                return NotFound("order Not found");
            }
            return Ok(debt);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DebtReceive receive)
        {
            var order = await _service.Create(receive);
            return CreatedAtAction(nameof(Create), order);

        }
    }
}
