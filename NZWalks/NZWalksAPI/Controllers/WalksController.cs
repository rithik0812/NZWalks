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
        private readonly IWalksDifficultyRepository walksDifficultyRepository;

        public WalksController(IWalkRepository walkRepository, IMapper mapper,
            IRegionRepository regionRepository, IWalksDifficultyRepository walksDifficultyRepository)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
            this.regionRepository = regionRepository;
            this.walksDifficultyRepository = walksDifficultyRepository;
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
            // validation checks 

            if (await ValidateAddWalksAsync(addWalkRequest) == false) 
            {
                return BadRequest(ModelState);
            }

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
            // validation checks 

            if (await ValidateUpdateWalksAsync(updateWalkRequest) == false)
            {
                return BadRequest(ModelState);
            }

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

        #region Private Methods

        // validation of Add walks endpoint : 
        private async Task<bool> ValidateAddWalksAsync(Models.DTO.AddWalksRequestDTO addWalksRequest)
        {
            if (addWalksRequest == null)
            {
                ModelState.AddModelError(nameof(addWalksRequest), "Request for adding a region cannot be null");

                return false;
            }

            if (string.IsNullOrWhiteSpace(addWalksRequest.Name))
            {
                ModelState.AddModelError(nameof(addWalksRequest.Name), $"{nameof(addWalksRequest.Name)} cannot be null/ empty / whitespace");
            }

            if (addWalksRequest.Length > 0)
            {
                ModelState.AddModelError(nameof(addWalksRequest.Length), $"{nameof(addWalksRequest.Length)} should be greater than 0");
            }

            // the region id must correspond to an actual region in the DB

            var DoesTheRegionExist = await regionRepository.GetSingleAsync(addWalksRequest.RegionID);
            if (DoesTheRegionExist == null) 
            {
                ModelState.AddModelError(nameof(addWalksRequest.RegionID), $"{addWalksRequest.RegionID} is invalid");
            }

            // the walkDiffiulty id must correspond to an actual walkDifficulty in the DB

            var DoesTheWalksDifficultyExist = await regionRepository.GetSingleAsync(addWalksRequest.WalkDifficultyID);
            if (DoesTheRegionExist == null)
            {
                ModelState.AddModelError(nameof(addWalksRequest.WalkDifficultyID), $"{addWalksRequest.WalkDifficultyID} is invalid");
            }

            // if any of the validation checks fail then return false
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            // if the all the validation chacks pass then return true
            return true;

        }

        // validation of Update walks endpoint : 
        private async Task<bool> ValidateUpdateWalksAsync(Models.DTO.UpdateWalksRequestDTO updateWalksRequest)
        {
            if (updateWalksRequest == null)
            {
                ModelState.AddModelError(nameof(updateWalksRequest), "Request for adding a region cannot be null");

                return false;
            }

            if (string.IsNullOrWhiteSpace(updateWalksRequest.Name))
            {
                ModelState.AddModelError(nameof(updateWalksRequest.Name), $"{nameof(updateWalksRequest.Name)} cannot be null/ empty / whitespace");
            }

            if (updateWalksRequest.Length > 0)
            {
                ModelState.AddModelError(nameof(updateWalksRequest.Length), $"{nameof(updateWalksRequest.Length)} should be greater than 0");
            }

            // the region id must correspond to an actual region in the DB

            var DoesTheRegionExist = await regionRepository.GetSingleAsync(updateWalksRequest.RegionID);
            if (DoesTheRegionExist == null)
            {
                ModelState.AddModelError(nameof(updateWalksRequest.RegionID), $"{updateWalksRequest.RegionID} is invalid");
            }

            // the walkDiffiulty id must correspond to an actual walkDifficulty in the DB

            var DoesTheWalksDifficultyExist = await regionRepository.GetSingleAsync(updateWalksRequest.WalkDifficultyID);
            if (DoesTheRegionExist == null)
            {
                ModelState.AddModelError(nameof(updateWalksRequest.WalkDifficultyID), $"{updateWalksRequest.WalkDifficultyID} is invalid");
            }

            // if any of the validation checks fail then return false
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            // if the all the validation chacks pass then return true
            return true;

        }

        #endregion

    }
}