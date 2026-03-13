using Microsoft.AspNetCore.Mvc;
using StrategyService.Dtos;
using StrategyService.Services;

namespace StrategyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StrategyController : ControllerBase
    {
        private readonly IStrategyService _strategyService;

        public StrategyController(IStrategyService strategyService)
        {
            _strategyService = strategyService;
        }

        // POST /api/strategy/calculate
        // Calcule et sauvegarde un plan de remboursement (Avalanche ou Snowball)
        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate([FromBody] CalculateStrategyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var plan = await _strategyService.CalculateAndSaveAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = plan.Id }, plan);
        }

        // GET /api/strategy/plans
        // Retourne tous les plans sauvegardés
        [HttpGet("plans")]
        public async Task<IActionResult> GetAll()
        {
            var plans = await _strategyService.GetAllAsync();
            return Ok(plans);
        }

        // GET /api/strategy/plans/{id}
        // Retourne un plan par son ID
        [HttpGet("plans/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plan = await _strategyService.GetByIdAsync(id);
            if (plan is null)
                return NotFound($"Plan with id {id} not found.");
            return Ok(plan);
        }

        // GET /api/strategy/plans/user/{userId}
        // Retourne tous les plans d'un utilisateur
        [HttpGet("plans/user/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var plans = await _strategyService.GetByUserIdAsync(userId);
            return Ok(plans);
        }

        // DELETE /api/strategy/plans/{id}
        // Supprime un plan
        [HttpDelete("plans/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _strategyService.DeleteAsync(id);
            if (!deleted)
                return NotFound($"Plan with id {id} not found.");
            return NoContent();
        }
    }
}
