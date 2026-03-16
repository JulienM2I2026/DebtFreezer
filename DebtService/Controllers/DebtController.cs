using DebtService.Dtos;
using DebtService.Models;
using DebtService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DebtService.Controllers
{
    [Route("api/v1/[controller]")]
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

        // GET /api/v1/Debt/paged?userId=...&page=1&pageSize=10&status=0&type=1&creditor=bank
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] DebtQueryDto query)
        {
            var result = await _service.GetPagedAsync(query);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            Console.WriteLine("ID: ");
            Console.WriteLine(id);
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

        // GET /api/v1/Debt/summary?userId=...
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] Guid userId)
        {
            var summary = await _service.GetDebtSummary(userId);
            return Ok(summary);
        }

        // GET /api/v1/Debt/by-month?userId=...
        [HttpGet("by-month")]
        public async Task<IActionResult> GetByMonth([FromQuery] Guid userId)
        {
            var result = await _service.GetDebtByMonth(userId);
            return Ok(result);
        }

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

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, [FromBody] UpdateDebtDto dto)
        {
            var result = await _service.Patch(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.Delete(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
