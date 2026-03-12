using Gateway.Dtos;
using Gateway.RestClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
