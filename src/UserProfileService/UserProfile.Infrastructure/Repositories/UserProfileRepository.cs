using AbstractionBlocks.Common.Infrastructure.Persistance;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using UserProfileService.Application.Interfaces;
namespace UserProfileService.Infrastructure.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly MongoDatabase<UserProfileService.Domain.Entities.UserProfile> _mongoDatabase;
        private readonly ILogger<UserProfileRepository> _logger;
        private IMongoCollection<UserProfileService.Domain.Entities.UserProfile> Collection => _mongoDatabase.Collection;
        public UserProfileRepository(
            MongoDatabase<UserProfileService.Domain.Entities.UserProfile> mongoDatabase,
            ILogger<UserProfileRepository> logger)
        {
            _mongoDatabase = mongoDatabase;
            _logger = logger;
        }
        public async Task<UserProfileService.Domain.Entities.UserProfile?> GetByIdAsync(Guid id)
        {
            var filter = Builders<UserProfileService.Domain.Entities.UserProfile>.Filter.Eq(x => x.Id, id);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<UserProfileService.Domain.Entities.UserProfile?> GetByUserIdAsync(Guid userId)
        {
            var filter = Builders<UserProfileService.Domain.Entities.UserProfile>.Filter.Eq(x => x.UserId, userId);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<bool> AddAsync(UserProfileService.Domain.Entities.UserProfile userProfile)
        {
            try
            {
                await Collection.InsertOneAsync(userProfile);
                _logger.LogInformation("UserProfile added: {UserId}", userProfile.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding UserProfile: {UserId}", userProfile.UserId);
                return false;
            }
        }
        public async Task<bool> UpdateAsync(UserProfileService.Domain.Entities.UserProfile userProfile)
        {
            try
            {
                var filter = Builders<UserProfileService.Domain.Entities.UserProfile>.Filter.Eq(x => x.Id, userProfile.Id);
                var result = await Collection.ReplaceOneAsync(filter, userProfile);
                _logger.LogInformation("UserProfile updated: {UserId}", userProfile.UserId);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating UserProfile: {UserId}", userProfile.UserId);
                return false;
            }
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var filter = Builders<UserProfileService.Domain.Entities.UserProfile>.Filter.Eq(x => x.Id, id);
                var result = await Collection.DeleteOneAsync(filter);
                _logger.LogInformation("UserProfile deleted: {Id}", id);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting UserProfile: {Id}", id);
                return false;
            }
        }
    }
}
