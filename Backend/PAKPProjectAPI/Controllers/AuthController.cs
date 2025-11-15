using PAKPProjectData;
using PAKPProjectServices;
using Microsoft.AspNetCore.Mvc;

namespace PAKPProjectAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthRepository authRepository, IRefreshTokenRepository refreshTokenRepository) : Controller
    {
        private readonly IAuthRepository _authRepository = authRepository;
        private readonly IRefreshTokenRepository _refreshTokenService = refreshTokenRepository;

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            await _authRepository.RegisterAsync(registerDto);
            return Ok("Register successful");
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDto)
        {
            await _authRepository.LoginAsync(loginDto);
            return Ok("Login successful");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> GenerateNewRefreshToken()
        {
            RefreshTokenResponseDTO newRefreshToken = await _refreshTokenService.GenerateNewRefreshTokenAsync();
            return Ok(newRefreshToken);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authRepository.Logout();
            return Ok("Logged out successfully");
        }
    }
}
