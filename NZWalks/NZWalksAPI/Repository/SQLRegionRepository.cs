using Microsoft.EntityFrameworkCore;
using NZWalksAPI.Data;
using NZWalksAPI.Models.Domain;

namespace NZWalksAPI.Repository
{
    public class SQLRegionRepository : IRegionRepository
    {
        // note the methods/async methods from the _nZWalksDbContext are inherited from the DbContext class we dont make them

        private NZWalksDBContext _nZWalksDbContext;

        public SQLRegionRepository(NZWalksDBContext nZWalksDBContext)
        {
            this._nZWalksDbContext = nZWalksDBContext;
        }

        public async Task<IEnumerable<Region>> GetAllAsync()
        {
            // returns null if no regions in DB

            return await _nZWalksDbContext.Regions.ToListAsync();
        }

        public async Task<Region> GetSingleAsync(Guid Id)
        {
            // returns null if it cant find the region in DB

            return await _nZWalksDbContext.Regions.FirstOrDefaultAsync(x => x.ID == Id);
        }

        public async Task<Region> AddRegionsAsync(Region region)
        {
            // set the region id in the api (rather than the client)
            // add the region to the DB then save changes to the DB
            // return region that was added

            region.ID= Guid.NewGuid();
            await _nZWalksDbContext.AddAsync(region);
            await _nZWalksDbContext.SaveChangesAsync();

            return region;
        }

        public async Task<Region> DeleteRegionAsync(Guid Id)
        {
            // FirstOrDefault method finds a region from DB by id. Returns null if it cant find Id in Db. 

            var region = await _nZWalksDbContext.Regions.FirstOrDefaultAsync(x => x.ID == Id);

            if (region == null) 
            {
                return null;
            }

            // Delete the region from the DB

            _nZWalksDbContext.Regions.Remove(region);

            await _nZWalksDbContext.SaveChangesAsync();

            return region;

        }

        public async Task<Region> UpdateRegionAsync(Guid Id, Region region)
        {
            // FirstOrDefault method finds a region from DB by id. Returns null if it cant find Id in Db. 

            var ExistingRegion = await _nZWalksDbContext.Regions.FirstOrDefaultAsync(x => x.ID == Id);

            if (ExistingRegion == null) 
            {
                return null;
            }

            ExistingRegion.Code= region.Code;
            ExistingRegion.Name= region.Name;
            ExistingRegion.Area= region.Area;
            ExistingRegion.Lat= region.Lat;
            ExistingRegion.Long= region.Long;
            ExistingRegion.Population= region.Population;

            await _nZWalksDbContext.SaveChangesAsync();

            return ExistingRegion;
        }
    }
}
