using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace PAKPProjectData
{
    public class User : BaseModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required, MinLength(10), MaxLength(200)]
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        [Required, MinLength(10), MaxLength(200)]
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        [JsonIgnore]
        public ICollection<RefreshToken> RefreshTokens { get; } = new List<RefreshToken>();

        public override T ToDto<T>()
        {
            CurrentUserDTO currentUser = new CurrentUserDTO()
            {
                ID = ID,
                Email = Email,
                Username = Username,
                PasswordHash = PasswordHash,
                PasswordSalt = PasswordSalt,
                DateCreated = DateCreated,
            };

            return (T)(object)currentUser;
        }
    }
}
