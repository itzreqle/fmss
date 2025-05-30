using Microsoft.AspNetCore.Mvc;
using Roza.AuthService.Models;
using Roza.AuthService.Services;

namespace Roza.AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _authService.RegisterAsync(model);
            return result.Succeeded ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authService.LoginAsync(model);
            return result.Succeeded ? Ok(new { result.AccessToken, result.RefreshToken }) : Unauthorized(result.Message);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] RefreshModel request)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            return result.Succeeded ? Ok(new { result.AccessToken, result.RefreshToken }) : Unauthorized(result.Message);
        }

    }
}
