using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAKPProjectData;
using PAKPProjectServices;

namespace PAKPProjectAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController(DataContext dataContext, IUserService userService) : Controller
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IUserService _userService = userService;

        [HttpGet("get-profile/{profileId}")]
        public async Task<ActionResult> GetProfile(int profileId)
        {
            try
            {
                UserProfile? profile = await _dataContext.UserProfiles.FindAsync(profileId);

                if (profile is null)
                {
                    return NotFound("Profile not found");
                }

                return Ok(new
                {
                    profile = profile.ToDto<UserProfileDTO>(),
                    method = ""
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    method = ""
                });
            }
        }

        [HttpPut("update-profile/{profileId}")]
        public async Task<ActionResult> UpdateProfile(int profileId, [FromBody] UserProfileUpdateDTO updateDto)
        {
            try
            {
                UserProfile? profile = await _dataContext.UserProfiles.FindAsync(profileId);

                if (profile is null)
                {
                    return NotFound("Profile not found");
                }

                profile.FirstName = updateDto.FirstName;
                profile.LastName = updateDto.LastName;
                profile.Bio = updateDto.Bio;
                profile.PhoneNumber = updateDto.PhoneNumber;
                profile.Address = updateDto.Address;
                profile.IsPublic = updateDto.IsPublic;

                await _dataContext.SaveChangesAsync();

                return Ok(new
                {
                    message = "Profile updated",
                    method = ""
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }

        [HttpPost("create-profile")]
        public async Task<ActionResult> CreateProfile([FromBody] CreateProfileDTO createProfileDto)
        {
            try
            {
                CurrentUserDTO currentUser = await _userService.GetCurrentUserAsync();
                UserProfile? existingProfile = await _dataContext.UserProfiles.FirstOrDefaultAsync(p => p.UserID == currentUser.ID);

                if (existingProfile is not null)
                {
                    return BadRequest(new
                    {
                        Error = "User already has a profile"
                    });
                }

                UserProfile profile = new UserProfile()
                {
                    FirstName = createProfileDto.FirstName,
                    LastName = createProfileDto.LastName,
                    Bio = createProfileDto.Bio,
                    PhoneNumber = createProfileDto.PhoneNumber,
                    Address = createProfileDto.Address,
                    IsPublic = createProfileDto.IsPublic,
                    UserID = currentUser.ID
                };

                _dataContext.UserProfiles.Add(profile);
                await _dataContext.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Profile created successfully",
                    ProfileId = profile.ID,
                    Profile = profile.ToDto<UserProfileDTO>()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }
    }
}