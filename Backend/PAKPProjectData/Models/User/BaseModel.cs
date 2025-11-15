using System.ComponentModel.DataAnnotations;

namespace PAKPProjectData
{
    public abstract class BaseModel
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public virtual T ToDto<T>() where T : class
        {
            throw new NotImplementedException("ToDto must be implemented by derived classes");
        }
    }
}
