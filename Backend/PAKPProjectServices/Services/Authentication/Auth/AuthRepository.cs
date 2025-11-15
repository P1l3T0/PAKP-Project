using PAKPProjectData;

namespace PAKPProjectServices
{
    public class AuthRepository(IAuthService authService, IUserService userService) : IAuthRepository
    {
        private readonly IAuthService _authService = authService;
        private readonly IUserService _userService = userService;

        public async Task RegisterAsync(RegisterDTO registerDto)
        {
            await _userService.ValidateUserAsync(registerDto);

            (byte[] hashedPassword, byte[] saltPassword) = _userService.HashPassword(registerDto);

            CurrentUserDTO currentUser = await _authService.RegisterAsync(registerDto, hashedPassword, saltPassword);

            await _authService.GenerateAuthResponse(currentUser);
        }

        public async Task LoginAsync(LoginDTO loginDto)
        {
            CurrentUserDTO currentUser = await _authService.LoginAsync(loginDto);

            await _authService.GenerateAuthResponse(currentUser);
        }

        public Task Logout()
        {
            _authService.Logout();

            return Task.CompletedTask;
        }
    }
}
