using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PAKPProjectData
{
    public class UserPost : BaseModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public bool IsPrivate { get; set; } = false;

        [Required]
        public int UserID { get; set; }

        [JsonIgnore]
        public User User { get; set; } = null!;

        public override T ToDto<T>()
        {
            UserPostDTO dto = new UserPostDTO()
            {
                ID = ID,
                Title = Title,
                Content = Content,
                IsPrivate = IsPrivate,
                UserID = UserID,
                DateCreated = DateCreated
            };

            return (T)(object)dto;
        }
    }
}