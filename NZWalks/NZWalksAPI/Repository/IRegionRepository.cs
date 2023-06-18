using NZWalksAPI.Models.Domain;

namespace NZWalksAPI.Repository
{
    public interface IRegionRepository
    {
        Task<IEnumerable<Region>> GetAllAsync();

        Task<Region> GetSingleAsync(Guid Id);

        Task<Region> AddRegionsAsync(Region region);

        Task<Region> DeleteRegionAsync(Guid Id);

        Task<Region> UpdateRegionAsync(Guid Id, Region region);

    }
}
