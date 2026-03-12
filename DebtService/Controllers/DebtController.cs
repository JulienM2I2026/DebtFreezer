using DebtService.Dtos;
using DebtService.Models;
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
        public async Task<IActionResult> GetAll([FromQuery] Guid userId)
        {
            return Ok(await _service.GetAllByUserId(userId));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _service.GetById(id));
        }

        /*
        [HttpGet]
        public async Task<IActionResult> GetByIdAndUserId([FromQuery] Guid userId, int DebtId)
        {
            return Ok(await _service.GetByUserAndDebtId(userId, DebtId));
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
        */

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DebtReceive receive)
        {
            Console.WriteLine("Service Create Debt");
            receive.RemainingAmount = receive.OriginalAmount;
            receive.Status = 0;
            var debt = await _service.Create(receive);
            return CreatedAtAction(nameof(Create), debt);

        }

        [HttpPost("{id:int}")]
        public async Task<IActionResult> UpdateDebtAmount(int id, [FromBody] double amount)
        {
            Console.WriteLine("Service UpdateDebtAmount");
            var debt = await _service.UpdateDebtAmount(id, amount);
            return CreatedAtAction(nameof(Create), debt);

        }
    }
}
