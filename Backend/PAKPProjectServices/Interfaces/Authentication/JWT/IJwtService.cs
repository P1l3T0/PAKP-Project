using System.IdentityModel.Tokens.Jwt;

namespace PAKPProjectServices
{
    public interface IJwtService
    {
        string GenerateAcessToken(int userID);
        string GenerateRefreshToken(int userID);
        string GetUserIdFromToken(JwtSecurityToken token);
        JwtSecurityToken Verify(string jwtToken);
    }
}
