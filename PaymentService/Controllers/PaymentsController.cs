using Microsoft.AspNetCore.Mvc;
using PaymentService.Dtos;
using PaymentService.Services;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/v1/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentsController(IPaymentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<PaymentDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PaymentDto>> GetById(int id)
    {
        var payment = await _service.GetByIdAsync(id);
        return payment is null ? NotFound() : Ok(payment);
    }

    [HttpGet("by-debt/{debtId:int}")]
    public async Task<ActionResult<List<PaymentDto>>> GetByDebtId(int debtId)
        => Ok(await _service.GetByDebtIdAsync(debtId));

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentDto dto)
    {
        // ApiController + DataAnnotations => ModelState auto 400
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<PaymentDto>> Update(int id, [FromBody] UpdatePaymentDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
