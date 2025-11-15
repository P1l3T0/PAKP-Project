using PAKPProjectData;

namespace PAKPProjectServices
{
    public interface IAuthRepository
    {
        Task RegisterAsync(RegisterDTO registerDto);
        Task LoginAsync(LoginDTO loginDto);
        Task Logout();
    }
}
