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

        [HttpGet("get-profile-safe/{profileId}")]
        public async Task<ActionResult> GetProfileSafe(int profileId)
        {
            try
            {
                UserProfile? profile = await _dataContext.UserProfiles.FindAsync(profileId);

                if (profile is null)
                {
                    return NotFound("Profile not found");
                }

                // Safe: Check if profile is public or belongs to current user
                CurrentUserDTO currentUser = await _userService.GetCurrentUserAsync();

                if (!profile.IsPublic && profile.UserID != currentUser.ID)
                {
                    return Forbid("You don't have permission to access this profile");
                }

                return Ok(new
                {
                    Profile = profile.ToDto<UserProfileDTO>(),
                    Method = "Safe with Authorization"
                });
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Error = "Access denied",
                    Method = "Safe with Authorization"
                });
            }
        }

        [HttpGet("get-profile-vulnerable/{profileId}")]
        public async Task<ActionResult> GetProfileVulnerable(int profileId)
        {
            try
            {
                // VULNERABLE: No authorization check - exposes private profiles
                UserProfile? profile = await _dataContext.UserProfiles.FindAsync(profileId);

                if (profile is null)
                {
                    return NotFound("Profile not found");
                }

                // VULNERABLE: Returns any profile regardless of privacy settings
                return Ok(new
                {
                    profile = profile.ToDto<UserProfileDTO>(),
                    method = "Vulnerable IDOR"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    method = "Vulnerable IDOR"
                });
            }
        }

        [HttpPut("update-profile-vulnerable/{profileId}")]
        public async Task<ActionResult> UpdateProfileVulnerable(int profileId, [FromBody] UserProfileUpdateDTO updateDto)
        {
            try
            {
                // VULNERABLE: Can update any user's profile
                UserProfile? profile = await _dataContext.UserProfiles.FindAsync(profileId);

                if (profile is null)
                {
                    return NotFound("Profile not found");
                }

                // VULNERABLE: No ownership check
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
                    method = "Vulnerable IDOR"
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

                // Check if user already has a profile
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