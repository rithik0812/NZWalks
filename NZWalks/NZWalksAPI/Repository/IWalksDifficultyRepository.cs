using NZWalksAPI.Models.Domain;

namespace NZWalksAPI.Repository
{
    public interface IWalksDifficultyRepository
    {
        Task<IEnumerable<WalkDifficulty>> GetAllAsync();
        Task<WalkDifficulty> GetAsync(Guid id);
        Task<WalkDifficulty> AddAsync(WalkDifficulty walkDifficulty);
        Task<WalkDifficulty> UpdateAsync(Guid id, WalkDifficulty walkDifficulty);
        Task<WalkDifficulty> DeleteAsync(Guid id);
    }
}
