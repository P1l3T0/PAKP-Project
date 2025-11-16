using PAKPProjectData;
using PAKPProjectServices;
using Microsoft.AspNetCore.Mvc;

namespace PAKPProjectAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthRepository authRepository, IRefreshTokenRepository refreshTokenRepository, IAuthService authService) : Controller
    {
        private readonly IAuthRepository _authRepository = authRepository;
        private readonly IRefreshTokenRepository _refreshTokenService = refreshTokenRepository;
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            await _authRepository.RegisterAsync(registerDto);
            return Ok("Register successful");
        }

        [HttpPost("login-safe")]
        public async Task<ActionResult> LoginSafe([FromBody] LoginDTO loginDto)
        {
            try
            {
                await _authRepository.LoginAsync(loginDto);
                return Ok(new { message = "Login successful", method = "Safe LINQ/EF" });
            }
            catch (Exception)
            {
                return BadRequest(new { error = "Login failed", method = "Safe LINQ/EF" });
            }
        }

        [HttpPost("login-vulnerable")]
        public async Task<ActionResult> LoginVulnerable([FromBody] LoginDTO loginDto)
        {
            try
            {
                CurrentUserDTO user = await _authService.LoginWithRawSqlAsync(loginDto);
                await _authService.GenerateAuthResponse(user);

                return Ok(new 
                { 
                    Message = "Login successful", 
                    User = user, 
                    Method = "Raw SQL" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    Error = ex.Message,
                    Details = "Raw SQL login failed",
                    Method = "Raw SQL"
                });
            }
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
