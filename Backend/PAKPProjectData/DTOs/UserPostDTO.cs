namespace PAKPProjectData
{
    public class UserPostDTO : BaseDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsPrivate { get; set; } = false;
        public int UserID { get; set; }
    }
}