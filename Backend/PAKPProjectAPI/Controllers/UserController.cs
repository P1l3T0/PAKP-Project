using Microsoft.AspNetCore.Mvc;
using PAKPProjectData;
using PAKPProjectServices;

namespace PAKPProjectAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserRepository userRepository) : Controller
    {
        private readonly IUserRepository _userRepository = userRepository;

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
    }
}
