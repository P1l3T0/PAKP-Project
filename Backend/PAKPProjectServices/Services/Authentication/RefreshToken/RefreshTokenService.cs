using PAKPProjectData;
using Microsoft.EntityFrameworkCore;

namespace PAKPProjectServices
{
    public class RefreshTokenService(DataContext dataContext) : IRefreshTokenService
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task<RefreshToken> AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            _dataContext.RefreshTokens.Add(refreshToken);
            await _dataContext.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
        {
            return await _dataContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken) ?? new RefreshToken();
        }

        public async Task<RefreshToken> GetRefreshTokenByUserIdAsync(int userID)
        {
            return await _dataContext.RefreshTokens.FirstOrDefaultAsync(t => t.UserID == userID && !t.IsRevoked) ?? new RefreshToken();
        }
    }
}
