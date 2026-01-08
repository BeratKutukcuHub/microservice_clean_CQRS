using UserProfileService.Domain.Entities;

namespace UserProfileService.Application.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfileService.Domain.Entities.UserProfile?> GetByIdAsync(Guid id);
        Task<UserProfileService.Domain.Entities.UserProfile?> GetByUserIdAsync(Guid userId);
        Task<List<UserProfileService.Domain.Entities.UserProfile>> GetAllAsync();
        Task<bool> AddAsync(UserProfileService.Domain.Entities.UserProfile userProfile);
        Task<bool> UpdateAsync(UserProfileService.Domain.Entities.UserProfile userProfile);
        Task<bool> DeleteAsync(Guid id);
    }
}
