using System.Text.Json.Serialization;

namespace PAKPProjectData
{
    public class RefreshToken : BaseModel
    {
        public int UserID { get; set; }
        public string Token { get; set; } = string.Empty;
        public bool IsRevoked { get; set; }
        public DateTime ExpiryDate { get; set; }

        [JsonIgnore]
        public User? User { get; set; }
    }
}
