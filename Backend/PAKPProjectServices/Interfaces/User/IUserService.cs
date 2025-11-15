using PAKPProjectData;

namespace PAKPProjectServices
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task<CurrentUserDTO> GetUserByEmailAsync(string email);
        Task<CurrentUserDTO> GetUserByUsernameAsync(string name);
        Task<CurrentUserDTO> GetUserByIdAsync(int userId);
        Task<User> GetUserEntityByIdAsync(int userId);
        Task<CurrentUserDTO> GetCurrentUserAsync();
        Task<int> GetCurrentUserID();
        Task ValidateUserAsync(RegisterDTO registerDto);
        Task<bool> UserExistsAsync(string email);
        bool AreFieldsEmpty(RegisterDTO registerDto);
        bool ValidateEmailAndPassword(string email, string password);
        (byte[] hashedPassword, byte[] saltPassword) HashPassword(RegisterDTO registerDto);
        bool CheckPassword(CurrentUserDTO currentUser, LoginDTO loginDto);
    }
}
