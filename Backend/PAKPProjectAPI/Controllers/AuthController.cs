using Microsoft.AspNetCore.Mvc;
using PAKPProjectData;

namespace PAKPProjectAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            return Ok("Register successful");
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDto)
        {
            return Ok("Login successful");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> GenerateNewRefreshToken()
        {
            return Ok("RefreshToken");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok("Logged out successfully");
        }
    }
}
