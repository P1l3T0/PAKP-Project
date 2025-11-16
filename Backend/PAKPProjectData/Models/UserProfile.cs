using System.ComponentModel.DataAnnotations;

namespace PAKPProjectData
{
    public class UserProfile : BaseModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public bool IsPublic { get; set; } = false;

        [Required]
        public int UserID { get; set; }

        public User User { get; set; } = null!;

        public override T ToDto<T>()
        {
            UserProfileDTO dto = new UserProfileDTO()
            {
                ID = ID,
                FirstName = FirstName,
                LastName = LastName,
                Bio = Bio,
                PhoneNumber = PhoneNumber,
                Address = Address,
                IsPublic = IsPublic,
                UserID = UserID,
                DateCreated = DateCreated
            };

            return (T)(object)dto;
        }
    }
}