using Microsoft.AspNetCore.Mvc;
using PAKPProjectData;
using PAKPProjectServices;

namespace PAKPProjectAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserRepository userRepository, IUserService userService, IAuthService authService) : Controller
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUserService _userService = userService;
        private readonly IAuthService _authService = authService;

        [HttpGet("get-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            if (string.IsNullOrEmpty(Request.Cookies["AccessToken"]))
            {
                return NoContent();
            }

            CurrentUserDTO currentUser = await _userRepository.GetCurrentUserAsync();

            return Ok(currentUser);
        }

        [HttpDelete("delete-current-user")]
        public async Task<IActionResult> DeleteCurrentUser()
        {
            await _userRepository.DeleteCurrentUserAsync();
            return Ok("User deleted");
        }

        [HttpGet("search-users-vulnerable")]
        public async Task<ActionResult> SearchUsersVulnerable([FromQuery] string search = "")
        {
            try
            {
                List<CurrentUserDTO> users = await _authService.SearchUsersWithRawSqlAsync(search);

                return Ok(new
                {
                    Users = users,
                    Count = users.Count,
                    Method = "Raw SQL"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message,
                    Query = search,
                    Method = "Raw SQL"
                });
            }
        }
    }
}
