using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PAKPProjectData;
using System.Data;

namespace PAKPProjectServices
{
    public class AuthService(IJwtService jwtService, IRefreshTokenService refreshTokenService, ICookieService cookieService, IUserService userService, DataContext dataContext) : IAuthService
    {
        private readonly IJwtService _jwtService = jwtService;
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
        private readonly ICookieService _cookieService = cookieService;
        private readonly IUserService _userService = userService;
        private readonly DataContext _dataContext = dataContext;

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
            if (!await _userService.UserExistsAsync(loginDto.Email)) throw new Exception("Invalid Username or Password");

            User currentUser = await _userService.GetUserByEmailAsync(loginDto.Email);

            if (!_userService.CheckPassword(currentUser, loginDto)) throw new Exception("Invalid Username or Password");

            return currentUser.ToDto<CurrentUserDTO>();
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

        public async Task<CurrentUserDTO> LoginWithRawSqlAsync(LoginDTO loginDto)
        {
            try
            {
                string query = $@"
                    SELECT Id, Email, Username, PasswordHash, PasswordSalt, DateCreated 
                    FROM Users 
                    WHERE Email = '{loginDto.Email}' AND Username IS NOT NULL AND Password = {loginDto.Password}".Trim();

                List<User> users = await _dataContext.Users
                    .FromSqlRaw(query)
                    .ToListAsync();

                if (!users.Any())
                {
                    throw new Exception("Invalid email or password");
                }

                User user = users.First();

                return new CurrentUserDTO
                {
                    ID = user.ID,
                    Email = user.Email,
                    Username = user.Username,
                    DateCreated = user.DateCreated
                };
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error occurred: {ex.Message}");
            }
        }

        public async Task<List<CurrentUserDTO>> SearchUsersWithRawSqlAsync(string searchTerm)
        {
            try
            {
                string query = $@"
                    SELECT Id, Email, Username, PasswordHash, PasswordSalt, DateCreated 
                    FROM Users 
                    WHERE Username LIKE '%{searchTerm}%' 
                    OR Email LIKE '%{searchTerm}%'".Trim();

                List<CurrentUserDTO> users = await _dataContext.Users
                    .FromSqlRaw(query)
                    .Select(u => new CurrentUserDTO
                    {
                        ID = u.ID,
                        Email = u.Email,
                        Username = u.Username,
                        DateCreated = u.DateCreated
                    })
                    .ToListAsync();

                return users;
            }
            catch (SqlException ex)
            {
                throw new Exception($"Search failed: {ex.Message}");
            }
        }
    }
}
