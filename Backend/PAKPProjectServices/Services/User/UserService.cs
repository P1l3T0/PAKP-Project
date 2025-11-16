using PAKPProjectData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;

namespace PAKPProjectServices
{
    public class UserService(DataContext dataContext, IJwtService jwtService, IHttpContextAccessor httpContextAccessor) : IUserService
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<User> CreateUserAsync(User user)
        {
            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetUserByEmailAsync(string email) => await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email) ?? throw new Exception("Invalid Email");

        public async Task<CurrentUserDTO> GetUserByUsernameAsync(string username) => (await _dataContext.Users.FirstOrDefaultAsync(u => u.Username == username))!.ToDto<CurrentUserDTO>() ?? throw new Exception("Invalid Username");

        public async Task<CurrentUserDTO> GetUserByIdAsync(int userID) => (await _dataContext.Users.FirstOrDefaultAsync(u => u.ID == userID))!.ToDto<CurrentUserDTO>() ?? throw new Exception("Invalid User ID");

        public async Task<User> GetUserEntityByIdAsync(int userID) => await _dataContext.Users.FirstOrDefaultAsync(u => u.ID == userID) ?? throw new Exception("Invalid User ID");

        public async Task<int> GetCurrentUserID() => (await GetCurrentUserAsync()).ID;

        public async Task DeleteUserAsync(int userID)
        {
            await _dataContext.Users.Where(u => u.ID == userID).ExecuteDeleteAsync();
            await _dataContext.SaveChangesAsync();
        }

        public async Task<CurrentUserDTO> GetCurrentUserAsync()
        {
            string jwt = _httpContextAccessor.HttpContext!.Request.Cookies["AccessToken"]!;

            if (string.IsNullOrEmpty(jwt)) throw new Exception("Access Token is not valid!");

            try
            {
                JwtSecurityToken token = _jwtService.Verify(jwt);
                int.TryParse(_jwtService.GetUserIdFromToken(token), out int userID);
                CurrentUserDTO currentUser = await GetUserByIdAsync(userID);

                return currentUser;
            }
            catch (Exception ex)
            {
                throw new Exception($"No user currently logged in! ${ex.Message}");
            }
        }

        public async Task ValidateUserAsync(RegisterDTO registerDto)
        {
            if (!AreFieldsEmpty(registerDto)) throw new Exception("Enter data in all fields");

            if (await UserExistsAsync(registerDto.Email)) throw new Exception("User already exists");

            if (!ValidateEmailAndPassword(registerDto.Email, registerDto.Password)) throw new Exception("Invalid Email or Password");
        }

        public bool AreFieldsEmpty(RegisterDTO registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.Email) || string.IsNullOrWhiteSpace(registerDto.Username) || string.IsNullOrWhiteSpace(registerDto.Password))
                return false;

            return true;
        }

        public async Task<bool> UserExistsAsync(string email) => await _dataContext.Users.AnyAsync(u => u.Email == email);

        public bool ValidateEmailAndPassword(string email, string password)
        {
            string emailRegExPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            string passwordRegExPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{10,}$";

            Regex emailRegex = new Regex(emailRegExPattern);
            Regex passwordRegex = new Regex(passwordRegExPattern);

            return emailRegex.IsMatch(email) && passwordRegex.IsMatch(password);
        }

        public (byte[] hashedPassword, byte[] saltPassword) HashPassword(RegisterDTO registerDTO)
        {
            using HMACSHA512 hmac = new HMACSHA512();

            byte[] hashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
            byte[] saltPassword = hmac.Key;

            return (hashedPassword, saltPassword);
        }

        public bool CheckPassword(User user, LoginDTO loginDto)
        {
            using HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);
            byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            if (!computedHash.SequenceEqual(user.PasswordHash)) return false;

            return true;
        }

        // Safe search method using EF LINQ (parameterized queries)
        public async Task<List<CurrentUserDTO>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<CurrentUserDTO>();
            }

            // Safe: EF automatically parameterizes these queries
            var users = await _dataContext.Users
                .Where(u => u.Username.Contains(searchTerm) || u.Email.Contains(searchTerm))
                .OrderBy(u => u.Username)
                .ToListAsync();

            return users.Select(u => u.ToDto<CurrentUserDTO>()).ToList();
        }
    }
}
