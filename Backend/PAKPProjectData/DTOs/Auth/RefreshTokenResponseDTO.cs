namespace PAKPProjectData
{
    public class RefreshTokenResponseDTO
    {
        public string NewAccessToken { get; set; } = string.Empty;
        public string NewRefreshToken { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }
}
