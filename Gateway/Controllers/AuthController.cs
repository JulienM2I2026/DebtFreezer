using Gateway.Dtos;
using Gateway.Filter;
using Gateway.RestClient;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly Client<AuthDto, LoginDto> clientLogin;
        private readonly Client<AuthDto, RegisterDto> clientRegister;
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
            var authServiceUrl = _config["ServiceUrls:AuthService"];
            clientLogin = new Client<AuthDto, LoginDto>($"{authServiceUrl}/api/Auth/");
            clientRegister = new Client<AuthDto, RegisterDto>($"{authServiceUrl}/api/Auth/");
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
            var response = await http.PostAsync("http://localhost:5130/api/Auth/forgot-password", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            return Content(responseBody, "application/json");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] object body)
        {
            using var http = new HttpClient();
            var json = System.Text.Json.JsonSerializer.Serialize(body);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await http.PostAsync("http://localhost:5130/api/Auth/reset-password", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return Content(responseBody, "application/json");
            return StatusCode((int)response.StatusCode, responseBody);
        }

        // GET /api/auth/me — retourne le profil de l'utilisateur connecté
        [HttpGet("me")]
        [RequireValidToken]
        public async Task<IActionResult> Me()
        {
            var rawToken = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
            if (string.IsNullOrWhiteSpace(rawToken))
                return Unauthorized();

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", rawToken);

            var response = await http.GetAsync("http://localhost:5130/api/Auth/me");
            if (!response.IsSuccessStatusCode)
                return Unauthorized();

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
