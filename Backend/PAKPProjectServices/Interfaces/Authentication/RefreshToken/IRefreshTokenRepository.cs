using PAKPProjectData;

namespace PAKPProjectServices
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshTokenResponseDTO> GenerateNewRefreshTokenAsync();
    }
}
