using System.Text.Json.Serialization;

namespace PAKPProjectData
{
    public class CurrentUserDTO : BaseDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

        [JsonIgnore]
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        [JsonIgnore]
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    }
}
