using ChallengeService.Dtos;
using ChallengeService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChallengeService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ChallengeController : ControllerBase
    {
        private readonly IChallengeService _challengeService;

        public ChallengeController(IChallengeService challengeService)
        {
            _challengeService = challengeService;
        }

        // POST /api/challenge
        // Créer un nouveau défi (le créateur est automatiquement inscrit)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChallengeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var challenge = await _challengeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = challenge.Id }, challenge);
        }

        // GET /api/challenge
        // Lister tous les défis
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var challenges = await _challengeService.GetAllAsync();
            return Ok(challenges);
        }

        // GET /api/challenge/{id}
        // Récupérer un défi par son ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var challenge = await _challengeService.GetByIdAsync(id);
            if (challenge is null)
                return NotFound($"Challenge with id {id} not found.");
            return Ok(challenge);
        }

        // POST /api/challenge/{id}/join
        // Rejoindre un défi
        [HttpPost("{id}/join")]
        public async Task<IActionResult> Join(int id, [FromBody] JoinChallengeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var participation = await _challengeService.JoinAsync(id, dto);
                return Ok(participation);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // POST /api/challenge/{id}/payment
        // Enregistrer un paiement dans le cadre du défi
        [HttpPost("{id}/payment")]
        public async Task<IActionResult> RecordPayment(int id, [FromBody] RecordChallengePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _challengeService.RecordPaymentAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // GET /api/challenge/{id}/progress
        // Progression collective du défi
        [HttpGet("{id}/progress")]
        public async Task<IActionResult> GetProgress(int id)
        {
            var progress = await _challengeService.GetProgressAsync(id);
            if (progress is null)
                return NotFound($"Challenge with id {id} not found.");
            return Ok(progress);
        }

        // GET /api/challenge/{id}/leaderboard
        // Classement des participants
        [HttpGet("{id}/leaderboard")]
        public async Task<IActionResult> GetLeaderboard(int id)
        {
            var leaderboard = await _challengeService.GetLeaderboardAsync(id);
            return Ok(leaderboard);
        }

        // DELETE /api/challenge/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _challengeService.DeleteAsync(id);
            if (!deleted) return NotFound($"Challenge with id {id} not found.");
            return NoContent();
        }
    }
}
