using PAKPProjectData;
using Microsoft.AspNetCore.Http;

namespace PAKPProjectServices 
{ 
    public class RefreshTokenRepository(IHttpContextAccessor httpContextAccessor, IRefreshTokenService refreshTokenService,
        IJwtService jwtService, ICookieService cookieService, IUserService userService) : IRefreshTokenRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly ICookieService _cookieService = cookieService;
        private readonly IUserService _userService = userService;

        public async Task<RefreshTokenResponseDTO> GenerateNewRefreshTokenAsync()
        {
            string refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["RefreshToken"]!;
            RefreshToken storedToken = await _refreshTokenService.GetRefreshTokenAsync(refreshToken);

            if (storedToken is null || storedToken.ExpiryDate < DateTime.Now || storedToken.IsRevoked)
            {
                throw new Exception("Invalid or expired refresh token.");
            }

            storedToken.IsRevoked = true;

            string newRefreshToken = _jwtService.GenerateRefreshToken(storedToken.UserID);
            string newAccessToken = _jwtService.GenerateAcessToken(storedToken.UserID);

            RefreshToken newRefreshTokenEntity = await _refreshTokenService.AddRefreshTokenAsync(new RefreshToken()
            {
                Token = newRefreshToken,
                ExpiryDate = DateTime.Now.AddDays(7),
                UserID = storedToken.UserID,
            });

            CurrentUserDTO currentUser = await _userService.GetUserByIdAsync(storedToken.UserID);

            _cookieService.DeleteCookie("RefreshToken");
            _cookieService.CreateCookie("AccessToken", newAccessToken);
            _cookieService.CreateCookie("RefreshToken", newRefreshToken);

            return new RefreshTokenResponseDTO()
            {
                NewAccessToken = newAccessToken,
                NewRefreshToken = newRefreshToken,
                ExpiresIn = 600,
                Username = currentUser.Username
            };
        }
    }
}
