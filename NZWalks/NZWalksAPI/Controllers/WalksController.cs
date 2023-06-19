using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Repository;
using NZWalksAPI.Models.Domain;
using System.Text;


namespace NZWalksAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalksController : Controller
    {
        private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;
        private readonly IRegionRepository regionRepository;
        //private readonly IWalkDifficultyRepository walkDifficultyRepository;

        public WalksController(IWalkRepository walkRepository, IMapper mapper,
            IRegionRepository regionRepository)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
            this.regionRepository = regionRepository;
            //this.walkDifficultyRepository = walkDifficultyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalksAsync()
        {
        // Fetch data from database - domain walks
        var walksDomain = await walkRepository.GetAllAsync();

            // Convert domain walks to DTO Walks
            var walksDTO = mapper.Map<List<Models.DTO.Walk>>(walksDomain);

            // Return response
            return Ok(walksDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkAsync")]
        public async Task<IActionResult> GetWalkAsync(Guid id)
        {
            // Get Walk Domain object from database
            var walkDomin = await walkRepository.GetAsync(id);

            // Convert Domain object to DTO
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomin);

            // Return response
            return Ok(walkDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkAsync([FromBody] Models.DTO.AddWalksRequestDTO addWalkRequest)
        {
            // Convert DTO to Domain Object
            var walkDomain = new Models.Domain.Walk
            {
                Length = addWalkRequest.Length,
                Name = addWalkRequest.Name,
                RegionID = addWalkRequest.RegionID,
                WalkDifficultyID = addWalkRequest.WalkDifficultyID
            };

            // Pass domain object to Repository to persist this
            walkDomain = await walkRepository.AddAsync(walkDomain);

            // Convert the Domain object back to DTO
            var walkDTO = new Models.DTO.Walk
            {
                ID = walkDomain.ID,
                Length = walkDomain.Length,
                Name = walkDomain.Name,
                RegionID = walkDomain.RegionID,
                WalkDifficultyID = walkDomain.WalkDifficultyID
            };

            // Send DTO response back to Client
            return CreatedAtAction(nameof(GetWalkAsync), new { id = walkDTO.ID }, walkDTO);
        }


        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id,
            [FromBody] Models.DTO.UpdateWalksRequestDTO updateWalkRequest)
        {

            // Convert DTO to Domain object
            var walkDomain = new Models.Domain.Walk
            {
                Length = updateWalkRequest.Length,
                Name = updateWalkRequest.Name,
                RegionID = updateWalkRequest.RegionID,
                WalkDifficultyID = updateWalkRequest.WalkDifficultyID
            };

            // Pass details to Repository - Get Domain object in response (or null)
            walkDomain = await walkRepository.UpdateAsync(id, walkDomain);

            // Handle Null (not found)
            if (walkDomain == null)
            {
                return NotFound();
            }

            // Convert back Domain to DTO
            var walkDTO = new Models.DTO.Walk
            {
                ID = walkDomain.ID,
                Length = walkDomain.Length,
                Name = walkDomain.Name,
                RegionID = walkDomain.RegionID,
                WalkDifficultyID = walkDomain.WalkDifficultyID
            };

            // Return Response
            return Ok(walkDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkAsync(Guid id)
        {
            // call Repository to delete walk
            var walkDomain = await walkRepository.DeleteAsync(id);

            if (walkDomain == null)
            {
                return NotFound();
            }

            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);

            return Ok(walkDTO);
        }

    }
}