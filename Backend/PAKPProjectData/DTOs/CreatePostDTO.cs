using System.ComponentModel.DataAnnotations;

namespace PAKPProjectData
{
    public class CreatePostDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public bool IsPrivate { get; set; } = false;
    }
}