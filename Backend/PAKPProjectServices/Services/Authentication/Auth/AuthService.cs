using PAKPProjectData;

namespace PAKPProjectServices
{
    public class AuthService(IJwtService jwtService, IRefreshTokenService refreshTokenService, ICookieService cookieService, IUserService userService) : IAuthService
    {
        private readonly IJwtService _jwtService = jwtService;
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
        private readonly ICookieService _cookieService = cookieService;
        private readonly IUserService _userService = userService;

        public async Task<CurrentUserDTO> RegisterAsync(RegisterDTO registerDto, byte[] hashedPassword, byte[] saltPassword)
        {
            User newUser = new User()
            {
                Email = registerDto.Email,
                Username = registerDto.Username,
                PasswordHash = hashedPassword,
                PasswordSalt = saltPassword
            };

            await _userService.CreateUserAsync(newUser);

            return newUser.ToDto<CurrentUserDTO>();
        }

        public async Task<CurrentUserDTO> LoginAsync(LoginDTO loginDto)
        {
            if (!await _userService.UserExistsAsync(loginDto.Email)) throw new Exception("User does not exist");

            CurrentUserDTO currentUser = await _userService.GetUserByEmailAsync(loginDto.Email);

            if (!_userService.CheckPassword(currentUser, loginDto)) throw new Exception("Invalid Password");

            return currentUser;
        }

        public async Task GenerateAuthResponse(CurrentUserDTO currentUser)
        {
            string accessToken = _jwtService.GenerateAcessToken(currentUser.ID);
            string refreshToken = _jwtService.GenerateRefreshToken(currentUser.ID);

            await _refreshTokenService.AddRefreshTokenAsync(new RefreshToken()
            {
                Token = refreshToken,
                ExpiryDate = DateTime.Now.AddDays(1),
                UserID = currentUser.ID
            });

            _cookieService.CreateCookie("AccessToken", accessToken);
            _cookieService.CreateCookie("RefreshToken", refreshToken);
        }

        public Task Logout()
        {
            _cookieService.DeleteCookie("AccessToken");
            _cookieService.DeleteCookie("RefreshToken");

            return Task.CompletedTask;
        }
    }
}
