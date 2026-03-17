using Gateway.Dtos;
using Gateway.Filter;
using Gateway.RestClient;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Gateway.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Client<AuthDto, LoginDto> clientLogin;
        private readonly Client<AuthDto, RegisterDto> clientRegister;
        private readonly string _authServiceUrl;

        public AuthController(IConfiguration config)
        {
            _authServiceUrl = $"{config["ServiceUrls:AuthService"]}/api/v1/Auth";
            clientLogin = new Client<AuthDto, LoginDto>($"{_authServiceUrl}/");
            clientRegister = new Client<AuthDto, RegisterDto>($"{_authServiceUrl}/");
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthDto>> Login([FromBody] LoginDto loginDto)
        {
            var result = await clientLogin.PostRequest("login", loginDto);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthDto>> Register([FromBody] RegisterDto registerDto)
        {
            var result = await clientRegister.PostRequest("register", registerDto);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] object body)
        {
            using var http = new HttpClient();
            var json = System.Text.Json.JsonSerializer.Serialize(body);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await http.PostAsync($"{_authServiceUrl}/forgot-password", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            return Content(responseBody, "application/json");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] object body)
        {
            using var http = new HttpClient();
            var json = System.Text.Json.JsonSerializer.Serialize(body);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await http.PostAsync($"{_authServiceUrl}/reset-password", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return Content(responseBody, "application/json");
            return StatusCode((int)response.StatusCode, responseBody);
        }

        // GET /api/v1/auth/me — retourne le profil de l'utilisateur connecté
        [HttpGet("me")]
        [RequireValidToken]
        public async Task<IActionResult> Me()
        {
            var rawToken = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
            if (string.IsNullOrWhiteSpace(rawToken))
                return Unauthorized();

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", rawToken);

            var response = await http.GetAsync($"{_authServiceUrl}/me");
            if (!response.IsSuccessStatusCode)
                return Unauthorized();

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        // GET /api/v1/auth/users — liste tous les utilisateurs (pour sélection challenger)
        [HttpGet("users")]
        [RequireValidToken]
        public async Task<IActionResult> GetAllUsers()
        {
            var rawToken = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

            using var http = new HttpClient();
            if (!string.IsNullOrWhiteSpace(rawToken))
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", rawToken);

            var response = await http.GetAsync($"{_authServiceUrl}/users");
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
