using AuthentificationService.Data;
using AuthentificationService.Dtos;
using AuthentificationService.Models;
using AuthentificationService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace AuthentificationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthDto>> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _userService.RegisterAsync(dto);
                return Created("", result);
            }
            catch (InvalidOperationException ex) when (ex.Message == "L'adresse email saisie est déjà utilisée")
            {
                return Conflict(new { message = "L'adresse email saisie est déjà utilisée" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthDto>> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _userService.LoginAsync(dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "L'adresse email ou le mot de passe saisi est incorrect" });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> Me()
        {
            var sub = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (sub is null || !Guid.TryParse(sub, out var userId))
                return Unauthorized();

            var me = await _userService.GetMeAsync(userId);
            if (me is null) return NotFound();

            return Ok(me);
        }

        // POST /api/auth/forgot-password
        // Génère un token de réinitialisation (valable 1h).
        // En production, ce token serait envoyé par email.
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var token = await _userService.ForgotPasswordAsync(dto);
            return Ok(new { message = token });
        }

        // POST /api/auth/reset-password
        // Réinitialise le mot de passe à partir du token reçu.
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                await _userService.ResetPasswordAsync(dto);
                return Ok(new { message = "Mot de passe réinitialisé avec succès." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("/ping")]
        [Authorize]
        public IActionResult Ping()
        {
            return Ok("Token Valide");
        }
    }
}