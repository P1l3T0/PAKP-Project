using System.ComponentModel.DataAnnotations;

namespace PAKPProjectData
{
    public class CreateProfileDTO
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public bool IsPublic { get; set; } = false;
    }
}