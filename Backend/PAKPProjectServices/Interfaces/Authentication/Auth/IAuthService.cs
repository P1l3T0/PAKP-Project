using PAKPProjectData;

namespace PAKPProjectServices
{
    public interface IAuthService
    {
        Task<CurrentUserDTO> RegisterAsync(RegisterDTO registerDto, byte[] hashedPassword, byte[] saltPassword);
        Task<CurrentUserDTO> LoginAsync(LoginDTO loginDto);
        Task Logout();
        Task GenerateAuthResponse(CurrentUserDTO currentUser);
    }
}
