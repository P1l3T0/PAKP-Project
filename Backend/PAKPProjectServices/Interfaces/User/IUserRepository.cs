using PAKPProjectData;

namespace PAKPProjectServices
{
    public interface IUserRepository
    {
        Task<CurrentUserDTO> GetCurrentUserAsync();
        Task DeleteCurrentUserAsync();
    }
}
